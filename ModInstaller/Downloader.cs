namespace ModInstaller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

public class Downloader
{

    private readonly HttpClient _client;
    private const string RepositoryUrl = "raw.githubusercontent.com";

    public Downloader()
    {
        var proxy = new WebProxy
        {
            Address = new Uri($"socks5://127.0.0.1:7890"),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,
        };
        // Create a client handler that uses the proxy
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
        };
        _client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        _client.Timeout = TimeSpan.FromSeconds(15);
    }

    public bool CheckNetworkConnection()
    {
        try
        {
            using var ping = new Ping();
            return ping.Send(RepositoryUrl, 3000)?.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
    internal async Task DownloadModFileAsync(string downloadurl, string zip_path)
    {
        try
        {
            var response = await _client.GetAsync(downloadurl);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var fileInfo = new FileInfo(zip_path);
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
        catch (Exception ex)
        {
            // logger.Report(ex);
        }
    }

    public async Task ZipDownloadAsync(string zip_path, string downloadurl, string md5_value)
    {
        try
        {
            var response = await _client.GetAsync(downloadurl);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var fileInfo = new FileInfo(zip_path);
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
        catch (Exception ex)
        {
            // _logger.Report(ex);
        }

        //string zip_path = Path.Combine(Application.Current.TemplateFolder, string.Format("{0}.zip", appId));
        //if (File.Exists(zip_path))
        //    File.Delete(zip_path);

        //progress_action.Invoke("开始下载安装包", 0, ModInstall_Error.None);

        //await DownloadModFileAsync(downloadurl, zip_path);


        //if (md5_value1 != md5_value)
        //{
        //    progress_action?.Invoke("下载 失败", 0, ModInstall_Error.Network_Err);
        //    return;
        //}

        //ZipInstall(appId, zip_path, destination, progress_action);
    }

    internal async Task<GameData[]?> CheckConfig(string repoMainURL)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(repoMainURL);
            if (response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                GameData[] data = JsonConvert.DeserializeObject<GameData[]>(text);

                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            // logger.Report(ex);
            return null;
        }
    }

    internal async Task<GameModSummaryData?> CheckGameModListConfig(string repoMainURL)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(repoMainURL);
            if (response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                GameModSummaryData data = JsonConvert.DeserializeObject<GameModSummaryData>(text);


                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            // logger.Report(ex);
            return null;
        }
    }
    internal async Task<ModItemData?> CheckGameModConfig(string repoMainURL)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(repoMainURL);
            if (response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                ModItemData data = JsonConvert.DeserializeObject<ModItemData>(text);


                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            // logger.Report(ex);
            return null;
        }
    }
}