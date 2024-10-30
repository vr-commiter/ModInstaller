using Micky5991.EventAggregator.Elements;

namespace ModInstaller
{
    internal class ConfigLoadedEvent : EventBase
    {
        public ConfigLoadedEvent()
        {
        }
    }
    internal class GameModConfigLoadedEvent : EventBase
    {
        private string _AppId;
        private Game_AllModDataConfig _cfg;

        public GameModConfigLoadedEvent(string appid, Game_AllModDataConfig cfg)
        {
            _AppId = appid;
            _cfg = cfg;
        }

        public string AppId
        {
            get { return _AppId; }
        }
        public Game_AllModDataConfig Config
        {
            get { return _cfg; }
        }
    }
}
