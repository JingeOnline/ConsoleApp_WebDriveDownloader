using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApp_WebDriveDownloader
{
    internal class HttpDownloader
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private int _MaxRetryCount = 5;
        private int _lastTryIndex = 1;

        private string getFileNameFromUri(string uri)
        {
            string s = HttpUtility.UrlDecode(uri);
            var parts = s.Split('/');
            return parts.Last();
        }
        public async Task DownloadFileAsync(string uri, string outputFilePath)
        {
            Console.WriteLine("Try=" + _lastTryIndex);
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += (sender, progress) =>
            {
                Console.Write("\r下载进度: "+progress);
            };
            using (var file = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await _httpClient.DownloadWithProgressAsync(uri, file, progress);
            }
            Console.WriteLine();
        }
        public async Task DownloadFileAndRetry(string uri, string outputFilePath)
        {
            try
            {
                Console.WriteLine();
                await DownloadFileAsync(uri, outputFilePath);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (_MaxRetryCount > 0)
                    {
                        _MaxRetryCount -= 1;
                        _lastTryIndex += 1;
                        await Task.Delay(5000);
                        await DownloadFileAndRetry(uri, outputFilePath);
                    }
                    else
                    {
                        Console.WriteLine("Always fail, exit.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
        public async Task Download(string uri, string folderPath)
        {
            string fileName = getFileNameFromUri(uri);
            string path = Path.Combine(folderPath, fileName);
            Console.WriteLine("\r\n开始下载: "+fileName);
            await DownloadFileAndRetry(uri, path);
        }
    }
}
