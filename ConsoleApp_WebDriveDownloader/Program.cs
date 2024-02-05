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
            //https://pan.clun.top/ 网站现在不支持，虽然下载链接都是添加一个/d/字段，但是该网站下载的get请求末尾都包含token。
            //示例：?sign=_bVRGXhtN2hT7z1Dwen2AKXsHB1DXDiB26ZJDSu9FCI=:0
            //string pageUrl = "https://pan.clun.top/115/%E8%B1%86%E7%93%A3%E5%BD%B1%E8%A7%86/%E8%B1%86%E7%93%A32022%E8%AF%84%E5%88%86%E6%9C%80%E9%AB%98%20%E8%8B%B1%E7%BE%8E%E7%BB%AD%E8%AE%A2%E5%89%A7/04.%E4%B8%87%E7%89%A9%E7%94%9F%E7%81%B5%201-3%E5%AD%A3%20-%20%E8%B1%86%E7%93%A39.5%E5%88%86/%E7%AC%AC%E4%BA%8C%E5%AD%A3%20%E3%80%90%20t188.ysepan.com%20%E3%80%91";
            string pageUrl = "https://al.chirmyram.com/tlv2/%E8%8B%B1%E5%89%A7/%E9%BB%91%E9%95%9C/%E9%BB%91%E9%95%9C%E7%AC%AC5%E5%AD%A3/%E9%BB%91%E9%95%9CBlack.Mirror.S05E03.Rachel.Jack.and.Ashley.Too.1080p.NF.WEB-DL.DDP5.1.x264-NTG";
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
