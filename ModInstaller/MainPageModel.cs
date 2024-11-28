namespace ModInstaller;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nito.AsyncEx;
using System.IO;
using System.Windows;

public class MainPageModel : ObservableObject
{
    private string _configFile => Path.Combine(AppShell.TemplateFolder, "config.txt");
    private bool _slientMode;

    public MainPageModel()
    {
        LoadModListDataCommand = new AsyncRelayCommand(GetModListDataCallback);
        SearchCmd = new AsyncRelayCommand<string>(SearchModCallback);
        InstallCmd = new AsyncRelayCommand(InstallCallback);
        var _eventAggregator = AppShell.Instance.getEventAggregator();
        _eventAggregator.Subscribe<ConfigLoadedEvent>(ConfigLoadedCallback);
        _eventAggregator.Subscribe<GameModConfigLoadedEvent>(GameModConfigLoadedCallback);

        _slientMode = File.Exists(_configFile);
    }

    internal void Init()
    {
        AsyncContext.Run(() => GetModListDataCallback());
    }


    private async void LoadGameAllMod(string appid)
    {
        await AppShell.Instance.LoadGameModConfig(appid);
        var FileName = AppShell.TemplateFolder + "config.txt";
        if (File.Exists(FileName))
        {
            await InstallCallback();
        }
    }
    private void GameModConfigLoadedCallback(GameModConfigLoadedEvent eventData)
    {
        ShowGameMod = true;

        if (eventData.Config != null)
        {
            CurrentGameModSummaryData = eventData.Config.Data;
            GameModListData = new List<GameModData>(eventData.Config.Data.Modlist); ;
        }
    }

    private void ConfigLoadedCallback(ConfigLoadedEvent eventData)
    {
        this.OperateStr = "Install";

        AppShell.Instance.Run((AppShellOptions options) =>
        {
            this.OperateStr = options.Operate;
            GameInstallPath = options.GamePath;
            GameDataPath = options.DataPath;

            if (options.AppId != null)
                LoadGameAllMod(options.AppId);
        });
    }

    private IList<GameModData> gameModListData;
    public IList<GameModData> GameModListData { get => gameModListData; set => SetProperty(ref gameModListData, value); }
    private GameModSummaryData _currentGameModSummaryData;
    public GameModSummaryData CurrentGameModSummaryData { get => _currentGameModSummaryData; set => SetProperty(ref _currentGameModSummaryData, value); }

    private string _GameInstallPath;
    public string GameInstallPath { get => _GameInstallPath; set => SetProperty(ref _GameInstallPath, value); }

    private string _dataPath;
    public string GameDataPath { get => _dataPath; set => SetProperty(ref _dataPath, value); }


    public IAsyncRelayCommand LoadModListDataCommand { get; }
    public IAsyncRelayCommand<string> SearchCmd { get; }
    public IAsyncRelayCommand InstallCmd { get; }

    public bool CanOperate { get => _canOperate; set => SetProperty(ref _canOperate, value); }
    private bool _canOperate = true;

    internal async Task GetModListDataCallback()
    {
    }

    private bool isShowGameMod;
    public bool ShowGameMod { get => isShowGameMod; set => SetProperty(ref isShowGameMod, value); }
    private string _operateStr;
    public string OperateStr { get => _operateStr; set => SetProperty(ref _operateStr, value); }

    private IList<ModItemData> modListData;
    public IList<ModItemData> ModListData { get => modListData; set => SetProperty(ref modListData, value); }
    internal async Task SearchModCallback(string text)
    {
    }

    internal async Task InstallCallback()
    {
        CanOperate = false;

        var installer = AppShell.Instance.getInstaller();
        if (OperateStr == "uninstall")
        {
            installer.ModDel(GameInstallPath, GameDataPath);
            Application.Current.Shutdown();
            return;
        }

        if (OperateStr == "reinstall")
        {
            installer.ModDel(GameInstallPath, GameDataPath);
        }

        var downloader = AppShell.Instance.getDownloader();
        GameModData modItem = GameModListData.First();
        string repoMainURL = string.Format("https://raw.githubusercontent.com/vr-commiter/HapticModList/refs/heads/main/{0}/{1}", CurrentGameModSummaryData.AppID, modItem.Json);
        var repoConfig = await downloader.CheckGameModConfig(repoMainURL);

        if (repoConfig == null)
        {
            CanOperate = true;
            return;
        }

        ModItemDataVersion downloadVer = repoConfig.VersionList.First();
        var zip_path = AppShell.TemplateFolder + "\\" + downloadVer.Md5Value + ".zip";
        var ex_path = AppShell.TemplateFolder + "\\" + downloadVer.Md5Value;
        await downloader.ZipDownloadAsync(zip_path, downloadVer.DownloadLink, downloadVer.Md5Value);
        string md5_value1 = "";
        if (File.Exists(zip_path))
            md5_value1 = Utils.CalculateMD5(zip_path);
        if (md5_value1 != downloadVer.Md5Value)
        {
            CanOperate = true;
            return;
        }
        installer.Unzip(zip_path, ex_path);
        installer.ZipInstallMod(ex_path, GameInstallPath);
        installer.ZipInstallApp(ex_path, GameDataPath);

        if (Directory.Exists(ex_path))
            Directory.Delete(ex_path, true);

        MessageBox.Show("Install Success");

        if (!string.IsNullOrEmpty(repoConfig.ReadmeLink))
        {
            System.Diagnostics.Process.Start("explorer.exe", repoConfig.ReadmeLink);
        }

        Application.Current.Shutdown();
    }
}
