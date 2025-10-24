using System.ComponentModel;
using UnityEngine;

namespace Push
{
#if EXILED
    public class Config : Exiled.API.Interfaces.IConfig
#else
    public class Config 
#endif
    {
        [Description("Enable or disable the push keybind. Default is true.")]
        public bool EnablePushKeybind { get; set; } = true;
        [Description("The keybind for pushing players. Default is Z.")]
        public KeyCode PushKeybind { get; set; } = KeyCode.Z;
        [Description("Enable or disable the pull keybind. Default is true.")]
        public bool EnablePullKeybind { get; set; } = true;
        [Description("The keybind for pulling players. Default is X.")]
        public KeyCode PullKeybind { get; set; } = KeyCode.X;
        [Description("The maximum push strength. Default is 5.")]
        public float MaxPushStrength { get; set; } = 5f;
        [Description("The range that players can reach when pushing/pulliing. Default is 5.")]
        public float PushPullRange { get; set; } = 5f;
        
        public Translations Translations { get; set; } = new Translations();
        
#if EXILED
        [Description("Enable or disable the plugin. Default is true.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Enable or disable debug logs. Default is false.")]
        public bool Debug { get; set; } = false;
#endif
        
    }
}