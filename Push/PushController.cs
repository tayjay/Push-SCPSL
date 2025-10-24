using System;
using System.Linq;
using DrawableLine;
using InventorySystem.Items.Armor;
using LabApi.Features.Wrappers;
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
        public ReferenceHub ReferenceHub => GetComponent<ReferenceHub>();

        public Player Player => Player.Get(ReferenceHub);
        
        public float LastPushTime { get; set; }

        private void Awake()
        {
            LastPushTime = Time.time;
        }

        public void PressPush()
        {
            Logger.Debug("PressPush called");
            if(Time.time-LastPushTime < 1f) return;
            LastPushTime = Time.time;
            
            Player target = GetLookedAtPlayer(Player, 5f);
            Logger.Debug("Found target");
            if (target == null) return;
            Logger.Debug("Target is not null");
            if (target.RoleBase is IFpcRole fpcRole)
            {
                MoveTarget(target, false);
            }
            Logger.Debug("Target push complete");
        }

        public void PressPull()
        {
            Logger.Debug("PressPull called");
            if(Time.time-LastPushTime < 1f) return;
            LastPushTime = Time.time;
            
            Player target = GetLookedAtPlayer(Player, 5f);
            if (target == null) return;
            if (target.RoleBase is IFpcRole fpcRole)
            {
                MoveTarget(target, true);
            }
        }


        public void MoveTarget(Player target, bool isPull)
        {
            Logger.Debug("MoveTarget called");
            if (target.RoleBase is IFpcRole fpcRole)
            {
                float resistance = 1f;
                if (target.Role.GetFaction() == Faction.SCP)
                {
                    resistance = 0.2f;
                }
                else if(target.Role.GetFaction() == Faction.FoundationStaff || target.Role.GetFaction() == Faction.FoundationEnemy)
                {
                    target.Inventory.TryGetBodyArmor(out BodyArmor bodyArmor);
                    if (bodyArmor != null)
                    {
                        resistance = bodyArmor.MovementSpeedMultiplier;
                    }
                }
                Vector3 forceDirection = Player.ReferenceHub.PlayerCameraReference.forward;
                float force = 5f * resistance;
                    
                fpcRole.FpcModule.Motor.ReceivedPosition = new RelativePosition(target.Position + forceDirection * force * (isPull ? -1f : 1f));
                    
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
                subcontroller._overlayAnimations[1].OnStarted();
                subcontroller._overlayAnimations[1].SendRpc();
                   
            }
        }
        
        public Player GetLookedAtPlayer(Player player,float range)
        {
            Logger.Debug("GetLookedAtPlayer called");
            var startPosition = player.ReferenceHub.PlayerCameraReference.position;
            var direction = player.ReferenceHub.PlayerCameraReference.forward;
            DrawableLines.IsDebugModeEnabled = true;
            DrawableLines.GenerateLine(1f,Color.red,startPosition, startPosition+ direction * range);
            
            var hits = Physics.RaycastAll(startPosition, direction, range).OrderBy(hit => hit.distance);

            foreach (var hit in hits)
            {
                Logger.Debug(hit.collider.gameObject.transform.root.gameObject.name);
                if (hit.collider.gameObject.transform.root.gameObject ==
                    player.GameObject.transform.root.gameObject)
                    continue;
                Logger.Debug("Checking non-self hit");
                if (hit.transform.root.gameObject == null)
                {
                    Logger.Debug("Null GameObject");
                    continue;
                }
                if (hit.transform.root.gameObject.TryGetComponent<ReferenceHub>(out var referenceHub))
                {
                    Logger.Debug("Found Player: "+referenceHub.nicknameSync._cleanDisplayName);
                    return Player.Get(referenceHub);
                }
                    
            }

            return null;
        }
    }
}