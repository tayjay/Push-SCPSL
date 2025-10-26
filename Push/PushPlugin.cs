using System;
using System.Linq;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace Push
{
    
    #if EXILED
    public class PushPlugin : Exiled.API.Features.Plugin<Config>
#else
    public class PushPlugin : Plugin<Config>
#endif
    
    {
        
        public static PushPlugin Instance { get; private set; }
        
        public SSPushSettings Settings { get; private set; }
        public EventsHandler EventsHandler { get; private set; }
        
#if EXILED
        public override void OnEnabled()
#else
        public override void Enable()
#endif
        {
#if EXILED
            if (LabApi.Loader.PluginLoader.EnabledPlugins.Any(plugin => plugin.Name == "Push.LabAPI"))
            {
                Exiled.API.Features.Log.Error("Both Push.EXILED and Push.LabAPI were detected. Disabling Push.EXILED, please remove Push.LabAPI plugin if you'd like to use this one instead.");
                return;
            }
#endif
            Instance = this;
            Settings = new SSPushSettings();
            EventsHandler = new EventsHandler();
            
            EventsHandler.Register();
            Settings.Activate();
            
#if EXILED
            base.OnEnabled();
#endif
        }

        

#if EXILED
        public override void OnDisabled()
#else
        public override void Disable()
#endif
        {
            if(EventsHandler != null)
                EventsHandler.Unregister();
            if(Settings != null)
                Settings.Deactivate();
            
            EventsHandler = null;
            Settings = null;
            Instance = null;
#if EXILED
            base.OnDisabled();
#endif
        }

        
        public override string Author { get; } = "TayTay";
        public override Version Version { get; } = typeof(PushPlugin).Assembly.GetName().Version;
        
#if EXILED
        public override string Name { get; } = "Push.EXILED";
            public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);
#else 
        public override string Name { get; } = "Push.LabAPI";
        public override string Description { get; } = "A plugin for LabApi that allows you to push other players a bit.";
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
#endif
        
        public string githubRepo = "tayjay/Push-SCPSL";
        
    }
    
    
    
    
    
}