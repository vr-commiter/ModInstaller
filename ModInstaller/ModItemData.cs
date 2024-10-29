using Newtonsoft.Json;

namespace ModInstaller;

public class ModItemDataVersion
{
    [JsonProperty("date_updated")] public string DateUpdated { get; set; }
    [JsonProperty("download_link")] public string DownloadLink { get; set; }
    [JsonProperty("md5_value")] public string Md5Value { get; set; }
    [JsonProperty("status")] public string Status { get; set; } // "test", "beta", "stable"
    [JsonProperty("version")] public string Version { get; set; }
}

public class ModItemData
{
    // Origin app ID: Steam_APPID or Origin_APPID or Epic_APPID 
    // how to find the app ID?
    // Steam: https://steamdb.info/
    // Origin: https://www.origin.com/store/
    // Epic: https://www.epicgames.com/store/
    [JsonProperty("appid")] public string AppID { get; set; }
    [JsonProperty("game_name")] public string GameName { get; set; }
    [JsonProperty("author_name")] public string AuthorName { get; set; }
    [JsonProperty("desc")] public string Desc { get; set; }
    [JsonProperty("website")] public string Website { get; set; }
    [JsonProperty("readme_link")] public string ReadmeLink { get; set; }
    [JsonProperty("version_list")] public ModItemDataVersion[] VersionList { get; set; }
}

public class GameData
{
    [JsonProperty("platform")] public string Platform { get; set; }
    [JsonProperty("appid")] public string AppID { get; set; } 
    [JsonProperty("game_name")] public string GameName { get; set; }
}
public class GameModSummaryData
{
    [JsonProperty("appid")] public string AppID { get; set; }
    [JsonProperty("game_name")] public string GameName { get; set; }
    [JsonProperty("mods")] public GameModData[] Modlist { get; set; }
    [JsonProperty("platform")] public string Platform { get; set; }
}

public class GameModData
{
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("status")] public string Status { get; set; } // "test", "beta", "stable"
    [JsonProperty("file")] public string Json { get; set; }
    [JsonProperty("author_name")] public string AuthorName { get; set; }
}