using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModInstaller
{
    public class Config
    {
        public GameData[] AllModItems;
        public string MainRepositoryUrl;
        public DateTime LastUpdated;
    }

    public class AllModItemData
    {
        public GameData[] Items;
    }

    public class Game_AllModDataConfig
    {
        public GameModSummaryData Data;
        public DateTime LastUpdated;
    }
}
