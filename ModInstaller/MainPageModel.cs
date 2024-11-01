namespace ModInstaller;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nito.AsyncEx;
using System.IO;
using System.Windows;

public class MainPageModel : ObservableObject
{
    public MainPageModel()
    {
        LoadModListDataCommand = new AsyncRelayCommand(GetModListDataCallback);
        SearchCmd = new AsyncRelayCommand<string>(SearchModCallback);
        InstallCmd = new AsyncRelayCommand<string>(InstallCallback);
        var _eventAggregator = AppShell.Instance.getEventAggregator();
        _eventAggregator.Subscribe<ConfigLoadedEvent>(ConfigLoadedCallback);
        _eventAggregator.Subscribe<GameModConfigLoadedEvent>(GameModConfigLoadedCallback);
    }

    internal void Init()
    {
        AsyncContext.Run(() => GetModListDataCallback());
    }


    private async void LoadGameAllMod(string appid)
    {
        await AppShell.Instance.LoadGameModConfig(appid);
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
        AppShell.Instance.Run((AppShellOptions options) =>
        {
            if (options.Operate == "install")
            {
                if (options.AppId != null)
                    LoadGameAllMod(options.AppId);
                GameInstallPath = options.InstallPath;
                GameDataPath = options.DataPath;
            }
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
    public IAsyncRelayCommand<string> InstallCmd { get; }

    internal async Task GetModListDataCallback()
    {
    }

    private bool isShowGameMod;
    public bool ShowGameMod { get => isShowGameMod; set => SetProperty(ref isShowGameMod, value); }

    private IList<ModItemData> modListData;
    public IList<ModItemData> ModListData { get => modListData; set => SetProperty(ref modListData, value); }

    internal async Task SearchModCallback(string text)
    {
    }

    internal async Task InstallCallback(string text)
    {
        var downloader = AppShell.Instance.getDownloader();
        GameModData modItem = GameModListData.First();
        string repoMainURL = string.Format("https://raw.githubusercontent.com/vr-commiter/HapticModList/refs/heads/main/{0}/{1}", CurrentGameModSummaryData.AppID, modItem.Json);
        var repoConfig = await downloader.CheckGameModConfig(repoMainURL);

        if (repoConfig == null)
        {
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
            return;
        }
        var installer = AppShell.Instance.getInstaller();
        installer.Unzip(zip_path, ex_path);
        installer.ZipInstallMod(ex_path, GameInstallPath);
        installer.ZipInstallApp(ex_path, GameDataPath);

        if (Directory.Exists(ex_path))
            Directory.Delete(ex_path, true);

        MessageBoxResult result = MessageBox.Show("Install Success");
        Application.Current.Shutdown();
    }
}
