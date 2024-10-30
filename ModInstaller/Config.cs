namespace ModInstaller
{
    public class Config
    {
        public GameData[] AllModItems;
        public string RepoEndpoint = "https://raw.githubusercontent.com/vr-commiter/HapticModList/refs/heads/main/";
        public DateTime LastUpdated;
    }


    public class Game_AllModDataConfig
    {
        public GameModSummaryData Data;
        public DateTime LastUpdated;
    }
}

