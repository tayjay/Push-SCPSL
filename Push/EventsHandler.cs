using LabApi.Events.Arguments.PlayerEvents;

namespace Push
{
    public class EventsHandler
    {
        public void OnSpawned(PlayerSpawnedEventArgs ev)
        {
            if(!ev.Player.GameObject.TryGetComponent<PushController>(out _))
                ev.Player.GameObject.AddComponent<PushController>();
        }

        public void Register()
        {
            LabApi.Events.Handlers.PlayerEvents.Spawned += OnSpawned;
        }

        public void Unregister()
        {
            LabApi.Events.Handlers.PlayerEvents.Spawned -= OnSpawned;
        }
    }
}