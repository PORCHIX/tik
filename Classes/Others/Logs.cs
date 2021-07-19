using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBotNS;

namespace LogsNS {
    class Logs {
        private static TelegramBotClient bot = TelegramBot.bot;
        public static async Task Getter(CallbackQueryEventArgs e, string channel ) 
        {
            using (StreamWriter sr = new("../../../Logs/logs.txt", true)) {
                sr.WriteLine($"@{e.CallbackQuery.From.Username}({e.CallbackQuery.From.Id}) - Успешно запостил видео в @{bot.GetChatAsync($"@{channel}").Result.Username}({bot.GetChatAsync($"@{channel}").Result.Id})");
            }
            var str = await System.IO.File.ReadAllLinesAsync("../../../Logs/NumberOfPostedTikTok.txt");
            using (StreamWriter sr = new("../../../Logs/NumberOfPostedTikTok.txt")) {
                sr.Write(Convert.ToInt64(str[0]) + 1);
            }
        }
        public static async Task Getter(string url) {
            using (StreamWriter sr = new("../../../Logs/Logs.txt", true)) {
                sr.WriteLine($"TikTok: {await LinkCorrector(url)}");
            }
        }
        public static async Task<string> LinkCorrector(string link) {
            return link.Substring(0, link.IndexOf('?'));
        }

    }
}
