namespace ModInstaller;

using CommandLine;
using Micky5991.EventAggregator.Interfaces;
using Micky5991.EventAggregator.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;

public class AppShellOptions
{
    [Option('o', "operate", Required = false)]
    public string Operate { get; set; }

    [Option('u', "download_url", Required = false)]
    public string DownloadUrl { get; set; }

    [Option('i', "appid", Required = false)]
    public string AppId { get; set; }

    [Option('p', "install_path", Required = false)]
    public string InstallPath { get; set; }
    [Option('d', "data_path", Required = false)]
    public string DataPath { get; set; }
}

public class AppShell
{
    public AppShell()
    {
        if (Instance != null)
        {
            throw new InvalidOperationException("Only one EventHandler instance allowed!");
        }

        Instance = this;
        _downloader = new Downloader();
        _installer = new Installer();
        _subscriptionLogger = new NullLogger<ISubscription>();
        _eventAggregator = new EventAggregatorService(_subscriptionLogger);
    }

    private ILogger<ISubscription> _subscriptionLogger;
    internal Micky5991.EventAggregator.Interfaces.IEventAggregator getEventAggregator()
    {
        return _eventAggregator;
    }

    public static AppShell Instance;
    private Installer _installer;
    internal Installer getInstaller()
    {
        return    _installer;
;
    }
    private Downloader _downloader;
    internal Downloader getDownloader()
    {
        return _downloader;
        ;
    }
    private Config _config;
    public static string RepoURL { get; set; }

    public static string TemplateFolder = Path.Combine(Path.GetTempPath(), "Vrhaptic_ModInstaller");

    private Micky5991.EventAggregator.Interfaces.IEventAggregator _eventAggregator;
    public static string SteamExePath()
    {
        return (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamExe", null);
    }

    public string CacheDownloadDirectory
    {
        get
        {
            var appDataRootPath = TemplateFolder + "downloads";
            return appDataRootPath;
        }
    }

    private AppShellOptions _runOptions;

    public void SetRunOptions(AppShellOptions options)
    {
        _runOptions = options;
    }

    public void Run(Action<AppShellOptions> action)
    {
        action?.Invoke(_runOptions);
    }

    public async Task LoadConfig(string repoMainURL, bool force = false)
    {
        try
        {
            Config config = null;
            string configPath = Path.Combine(TemplateFolder, "data.json");
            if(!Directory.Exists(TemplateFolder ))
                Directory.CreateDirectory(TemplateFolder);

            bool update = !File.Exists(configPath);
            if (File.Exists(configPath))
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
                if (config == null || DateTime.Now > config.LastUpdated.AddHours(1))
                    update = true;
            }

            if (update)
            {
                config = new Config();
                var repoConfig = await _downloader.CheckConfig(repoMainURL);

                if (repoConfig == null)
                {
                    _config = new Config();
                    return;
                }

                config.AllModItems = repoConfig;
                config.LastUpdated = DateTime.Now;

                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
                _config = config;
            }
        }
        catch
        {
        }
        _eventAggregator.Publish<ConfigLoadedEvent>(new ConfigLoadedEvent());
    }

    public async Task LoadGameModConfig(string appid)
    {
            Game_AllModDataConfig config = null;
        try
        {
            string repoMainURL = string.Format("https://raw.githubusercontent.com/vr-commiter/HapticModList/refs/heads/main/{0}/index.json", appid);

            string configPath = Path.Combine(TemplateFolder, appid + "_data.json");
            if (!Directory.Exists(TemplateFolder))
                Directory.CreateDirectory(TemplateFolder);

            bool update = !File.Exists(configPath);
            if (File.Exists(configPath))
            {
                config = JsonConvert.DeserializeObject<Game_AllModDataConfig>(File.ReadAllText(configPath));
                if (config == null || DateTime.Now > config.LastUpdated.AddHours(1))
                    update = true;
            }

            if (update)
            {
                config = new Game_AllModDataConfig();
                var repoConfig = await _downloader.CheckGameModListConfig(repoMainURL);

                if (repoConfig == null)
                {
                    config = new Game_AllModDataConfig();
                    return;
                }

                config.Data = repoConfig;
                config.LastUpdated = DateTime.Now;

                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
            }
        }
        catch
        {
        }
        _eventAggregator.Publish<GameModConfigLoadedEvent>(new GameModConfigLoadedEvent(appid, config));
    }
}