using System.Net;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace TTDownloaderNS {
    class TTDownloader {
        public static async Task DownloadOld (string ttUrl, string videoPath) {
            string htmlSourceCode;
            string url = "https://www.tiktokdownloader.org/check.php?v=" + ttUrl;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            StreamReader sr = new(response.GetResponseStream());
            response.Dispose();
            htmlSourceCode = await sr.ReadToEndAsync();
            sr.Dispose();
            string[] subs = htmlSourceCode.Split('"');
            url = subs[7];

            
            WebClient webClient = new();
            string videoId = url.Remove(0, 44);
            Uri rightUrl = new ("https://www.tiktokdownloader.org/d.php?h=" + videoId);
            await webClient.DownloadFileTaskAsync(rightUrl, videoPath);
            webClient.Dispose();
        }
        public static async Task Download (string ttUrl, string videoPath) {
            WebClient client = new ();
            Uri uri = new ($"https://www.tiktokdownloader.org/check.php?v={ttUrl}");
            var htmlCode = await client.DownloadStringTaskAsync(uri);
            if (!htmlCode.Contains("We could not download this tiktok video")) {
                string videoId = htmlCode.Substring((htmlCode.IndexOf("=") + 1),htmlCode.IndexOf("message")-htmlCode.IndexOf("=")-9);
                uri = new($"https://www.tiktokdownloader.org/d.php?h={videoId}");
                await client.DownloadFileTaskAsync(uri, videoPath);
                client.Dispose();
            } else { client.Dispose(); }
            
        }
    }
}
