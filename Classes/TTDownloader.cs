using System.Net;
using System.Threading.Tasks;
using System.IO;
using System;

namespace TTDownloaderNS {
    class TTDownloader {
        public static async Task Download (string ttUrl, string videoPath) {
            string htmlSourceCode;
            string url = "https://www.tiktokdownloader.org/check.php?v=" + ttUrl;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            using (Stream stream = response.GetResponseStream()) {
                using (StreamReader reader = new StreamReader(stream)) {
                    htmlSourceCode = reader.ReadToEnd();
                    if (htmlSourceCode.IndexOf("https") == -1) { Console.WriteLine(htmlSourceCode); }
                    string[] subs = htmlSourceCode.Split('"');
                    url = subs[7];
                }
            }
            response.Close();
            WebClient webClient = new WebClient();
            string videoId = url.Remove(0, 44);
            string rightUrl = "https://www.tiktokdownloader.org/d.php?h=" + videoId;
            webClient.DownloadFile(rightUrl, videoPath);
        }
    }
}
