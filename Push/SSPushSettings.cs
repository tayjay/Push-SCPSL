using System.Linq;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace Push
{
    public class SSPushSettings
    {
        private SSKeybindSetting pushKeybind;
        private SSKeybindSetting pullKeybind;

        public void Activate()
        {
            
            string pushName = (PushPlugin.Instance.Config.EnablePushKeybind ? $"{PushPlugin.Instance.Config.Translations.SSPushLabel}" : "[REDACTED]");
            string pullName =  (PushPlugin.Instance.Config.EnablePullKeybind ? $"{PushPlugin.Instance.Config.Translations.SSPullLabel}" : "[REDACTED]");
            
            
            pushKeybind = new SSKeybindSetting(null,pushName, PushPlugin.Instance.Config.DefaultPushKeybind,hint:PushPlugin.Instance.Config.Translations.SSPushHint);
            pullKeybind = new SSKeybindSetting(null,pullName, PushPlugin.Instance.Config.DefaultPullKeybind,hint:PushPlugin.Instance.Config.Translations.SSPullHint);

            
            var settings = new ServerSpecificSettingBase[3]
            {
                new SSGroupHeader(PushPlugin.Instance.Config.Translations.SSGroupLabel),
                pushKeybind,
                pullKeybind
            };
            
            if(ServerSpecificSettingsSync.DefinedSettings == null)
                ServerSpecificSettingsSync.DefinedSettings = settings;
            else
                ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Concat(settings).ToArray();
            ServerSpecificSettingsSync.SendToAll();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ProcessUserInput;
        }
        
        public void Deactivate()
        {
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ProcessUserInput;
        }
        
        private void ProcessUserInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (PushPlugin.Instance.Config.EnablePushKeybind && setting.SettingId == pushKeybind.SettingId && (setting is SSKeybindSetting kb && kb.SyncIsPressed))
            {
                if(sender?.gameObject?.TryGetComponent<PushController>(out var controller) == true)
                {
                    controller.PressPush();
                }
            } else if (PushPlugin.Instance.Config.EnablePullKeybind && setting.SettingId == pullKeybind.SettingId && (setting is SSKeybindSetting kb2 && kb2.SyncIsPressed))
            {
                if(sender?.gameObject?.TryGetComponent<PushController>(out var controller) == true)
                {
                    controller.PressPull();
                }
            }
        }
    }
}