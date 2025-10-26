using System;
using System.Collections.Generic;
using System.Linq;
using DrawableLine;
using InventorySystem.Items.Armor;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.OverlayAnims;
using RelativePositioning;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace Push
{
    public class PushController : MonoBehaviour
    {
        public float CooldownTime => PushPlugin.Instance.Config.PushPullCooldown;
        public float MaxStrength => PushPlugin.Instance.Config.MaxStrength;
        public float Range => PushPlugin.Instance.Config.PushPullRange;
        
        public ReferenceHub ReferenceHub => GetComponent<ReferenceHub>();

        public Player Player => Player.Get(ReferenceHub);
        
        public float LastPushTime { get; set; }

        private void Awake()
        {
            LastPushTime = Time.time;
        }

        public void PressPush()
        {
            try
            {
                Logger.Debug("PressPush called",PushPlugin.Instance.Config.Debug);
                if(Player.IsDisarmed) return;
                if(Time.time-LastPushTime < CooldownTime) return;
                LastPushTime = Time.time;
            
                Player target = GetLookedAtPlayer(Player, Range);
                if (target == null) return;
                if (target.RoleBase is IFpcRole fpcRole)
                {
                    MoveTarget(target, false);
                }
                Logger.Debug("Target push complete",PushPlugin.Instance.Config.Debug);
            } catch (Exception e)
            {
                Logger.Error("Error in PressPush: " + e);
            }
            
        }

        public void PressPull()
        {
            try
            {
                Logger.Debug("PressPull called",PushPlugin.Instance.Config.Debug);
                if(Player.IsDisarmed) return;
                if(Time.time-LastPushTime < CooldownTime) return;
                LastPushTime = Time.time;
            
                Player target = GetLookedAtPlayer(Player, Range);
                if (target == null) return;
                if (target.RoleBase is IFpcRole fpcRole)
                {
                    MoveTarget(target, true);
                }
            } catch (Exception e)
            {
                Logger.Error("Error in PressPull: " + e);
            }
        }


        public void MoveTarget(Player target, bool isPull)
        {
            Logger.Debug("MoveTarget called",PushPlugin.Instance.Config.Debug);
            if (target.RoleBase is IFpcRole fpcRole)
            {
                Logger.Debug("Target is FPC role",PushPlugin.Instance.Config.Debug);
                float factor = 1f;
                if (target.Role.GetFaction() == Faction.SCP)
                {
                    factor = 0.5f;
                }
                else if(target.Role.IsAlive())
                {
                    
                    if (target.Inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                    {
                        switch (bodyArmor.ItemTypeId)
                        {
                            case ItemType.ArmorLight:
                                factor = 0.7f;
                                break;
                            case ItemType.ArmorCombat:
                                factor = 0.6f;
                                break;
                            case ItemType.ArmorHeavy:
                                factor = 0.5f;
                                break;
                        }
                    }
                }
                Vector3 forceDirection = Player.ReferenceHub.PlayerCameraReference.forward.NormalizeIgnoreY();
                float force = MaxStrength * factor;
                Logger.Debug("Applying force: " + force + " factor: " + factor,PushPlugin.Instance.Config.Debug);
                if(target.IsNpc)
                    fpcRole.FpcModule.Motor.ReceivedPosition = new RelativePosition(target.Position + forceDirection.NormalizeIgnoreY() * force * (isPull ? -1f : 1f));
                WaypointToy Waypoint = WaypointToy.Create(target.Position);
                Waypoint.BoundsSize = new Vector3(0.2f, 0.1f, 0.2f);
                Timing.RunCoroutine(DragCoroutine(Waypoint, target.Position + forceDirection * force * (isPull ? -1f : 1f), 0.3f));
                
                OverlayAnimationsSubcontroller subcontroller;
                if (!(ReferenceHub.roleManager.CurrentRole is IFpcRole currentRole) ||
                    !(currentRole.FpcModule.CharacterModelInstance is AnimatedCharacterModel
                        characterModelInstance) ||
                    !characterModelInstance.TryGetSubcontroller<OverlayAnimationsSubcontroller>(out subcontroller))
                {
                    // Non-animated character model speaking
                    return;
                }
                // Hardcoded value, this SearchCompleteAnimation in the array
                Logger.Debug("Animating player overlay",PushPlugin.Instance.Config.Debug);
                subcontroller._overlayAnimations[1].OnStarted();
                subcontroller._overlayAnimations[1].SendRpc();
                   
            }
        }
        
        public IEnumerator<float> DragCoroutine(WaypointToy waypoint, Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = waypoint.Position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                waypoint.Position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Timing.DeltaTime;
                yield return Timing.WaitForOneFrame;
            }

            waypoint.Position = targetPosition;
            waypoint.Destroy();
        }
        
        public Player GetLookedAtPlayer(Player player,float range)
        {
            Logger.Debug("GetLookedAtPlayer called",PushPlugin.Instance.Config.Debug);
            var startPosition = player.ReferenceHub.PlayerCameraReference.position;
            var direction = player.ReferenceHub.PlayerCameraReference.forward;
            if (PushPlugin.Instance.Config.Debug)
            {
                DrawableLines.IsDebugModeEnabled = true;
                DrawableLines.GenerateLine(1f,Color.red,startPosition, startPosition+ direction * range);
            }
            
            var hits = Physics.RaycastAll(startPosition, direction, range).OrderBy(hit => hit.distance);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.transform.root.gameObject ==
                    player.GameObject.transform.root.gameObject)
                    continue;
                if (hit.transform.root.gameObject == null)
                {
                    continue;
                }
                if (hit.transform.root.gameObject.TryGetComponent<ReferenceHub>(out var referenceHub))
                {
                    return Player.Get(referenceHub);
                }
                    
            }

            return null;
        }
        
    }
}