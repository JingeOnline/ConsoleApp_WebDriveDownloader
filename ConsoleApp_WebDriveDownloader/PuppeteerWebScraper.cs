using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_WebDriveDownloader
{
    public class PuppeteerWebScraper
    {
        public async Task<List<string>> getDownloadUrls(string webPageUrl)
        {
            using var browserFetcher = new BrowserFetcher();
            {
                await browserFetcher.DownloadAsync();
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });
                using var page = await browser.NewPageAsync();
                {
                    await page.GoToAsync(webPageUrl);
                    //await Console.Out.WriteLineAsync(await page.GetContentAsync());
                    await page.WaitForSelectorAsync("a.chakra-linkbox__overlay");
                    var elements = await page.QuerySelectorAllAsync("a.chakra-linkbox__overlay");
                    Console.WriteLine("Find elements: " + elements?.Count());
                    List<string> urls = new List<string>();
                    foreach (var element in elements)
                    {
                        var obj = await element.GetPropertyAsync("href");
                        urls.Add(obj.RemoteObject.Value.ToString());
                    }
                    return urls;
                }
            }
        }
    }
}
