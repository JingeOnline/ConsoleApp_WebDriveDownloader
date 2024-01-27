namespace ConsoleApp_WebDriveDownloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await DownloadFiles(await GetFileUrlsFromPage());
        }

        static async Task DownloadFiles(List<string> urls)
        {
            string outputFolderPath = "D:\\下载文件夹";
            foreach (string uri in urls)
            {
                await new HttpDownloader().Download(uri, outputFolderPath);
            }
            Console.WriteLine("Done");
        }

        //目前只支持https://al.chirmyram.com/，其他类似的站点没有测试过
        static async Task<List<string>> GetFileUrlsFromPage()
        {
            string pageUrl = "https://al.chirmyram.com/tlv2/%E8%8B%B1%E5%89%A7/%E9%BB%91%E9%95%9C/%E9%BB%91%E9%95%9C%E7%AC%AC5%E5%AD%A3/%E9%BB%91%E9%95%9CBlack.Mirror.S05E02.Smithereens.1080p.NF.WEB-DL.DDP5.1.x264-NTG";
            PuppeteerWebScraper scraper = new PuppeteerWebScraper();
            List<string> urls= await scraper.getDownloadUrls(pageUrl);
            Console.WriteLine("Urls Find: "+urls?.Count);
            List<string> downloadUrls = new List<string>();
            foreach(string url in urls)
            {
                string downloadUrl=url.Replace("https://al.chirmyram.com/", "https://al.chirmyram.com/d/", StringComparison.Ordinal);
                Console.WriteLine(downloadUrl);
                downloadUrls.Add(downloadUrl);
            }
            Console.WriteLine();
            return downloadUrls;
        }
    }
}
