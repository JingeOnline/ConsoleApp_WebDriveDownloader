using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_WebDriveDownloader
{
    public static class HttpClientProgressExtensions
    {
        public static async Task DownloadWithProgressAsync(this HttpClient client, string requestUrl, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                Console.WriteLine("ResponseCode=" + response.StatusCode);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(response.Content.Headers.ContentType?.MediaType); 
                if(response.Content.Headers.ContentType?.MediaType=="application/json")
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    throw new HttpRequestException();
                }
                var contentLength = response.Content.Headers.ContentLength;
                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    // no progress... no contentLength... very sad
                    if (progress is null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }
                    // Such progress and contentLength much reporting Wow!
                    await download.CopyToAsync(destination, 1024 * 100, progress, contentLength.Value, cancellationToken);
                }
            }
        }

        static float GetProgressPercentage(long totalBytes, long currentBytes)
        {
            return ((float)totalBytes / currentBytes) * 100f;
        }

        static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<float> progress, long fileTotalBytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            DateTime startTime = DateTime.Now;
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new InvalidOperationException($"'{nameof(source)}' is not readable.");
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new InvalidOperationException($"'{nameof(destination)}' is not writable.");

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                //这里添加了一个时间限制，每1秒报告一次，否则每次缓冲16KB就会报告一次，过于频繁，而且修改缓冲区大小是无效的。
                if (DateTime.Now - startTime >= new TimeSpan(0, 0, 1)||totalBytesRead == fileTotalBytes)
                {
                    progress.Report(GetProgressPercentage(totalBytesRead, fileTotalBytes));
                    startTime= DateTime.Now;
                }
            }
        }
    }
}
