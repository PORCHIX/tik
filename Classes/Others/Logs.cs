using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Globalization;
using static TelegramBotNS.TelegramBot;
using Telegram.Bot.Types;

namespace LogsNS {
    class Logs {
        private const string logsFilePath = "../../../Logs/logs.txt";
        private static bool IsTelegramBotOwner(long userId) { return telegramBotOwnerId == userId; }
        public static void GetLogAboutPostToChannel(CallbackQueryEventArgs e, string channel) {
            if (IsTelegramBotOwner(e.CallbackQuery.From.Id)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] @{e.CallbackQuery.From.Username}({e.CallbackQuery.From.Id}) - Успешно запостил видео в @{bot.GetChatAsync($"@{channel}").Result.Username}({bot.GetChatAsync($"@{channel}").Result.Id})");
            sr.Close();
        }
        public static void GetErrorLogAboutPostToChannel (CallbackQueryEventArgs e, string channel) {
            if (IsTelegramBotOwner(e.CallbackQuery.From.Id)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] @{e.CallbackQuery.From.Username}({e.CallbackQuery.From.Id}) - Не смог запостить видео в @{bot.GetChatAsync($"@{channel}").Result.Username}({bot.GetChatAsync($"@{channel}").Result.Id})");
            sr.Close();
        }
        public static void GetLogAboutTikTokLink(long userId ,string url) {
            if (IsTelegramBotOwner(userId)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] @{bot.GetChatAsync(userId).Result.Username}({bot.GetChatAsync(userId).Result.Id}) успешно отправил в чат бота TikTok: {LinkCorrector(url)}");
            sr.Close();

        }
        public static string LinkCorrector(string link) {
            try {
                if (link.Contains("vm.tiktok.com")) { return link; } else { return link.Substring(0, link.IndexOf('?')); }
            } catch { return link; }
        }


        public static void GetLogAboutAddingChannelToUserFile(Message msg) {
            if (IsTelegramBotOwner(msg.Chat.Id)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] @{bot.GetChatAsync(msg.Chat.Id).Result.Username}({bot.GetChatAsync(msg.Chat.Id).Result.Id}) добавил бота: {msg.Text}");
            sr.Close();
        }

        public static void GetErrorLogAboutTTDownloader(Message msg) {
            if (IsTelegramBotOwner(msg.Chat.Id)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] У пользователя @{bot.GetChatAsync(msg.Chat.Id).Result.Username}({bot.GetChatAsync(msg.Chat.Id).Result.Id}) возникла ошибка при скачивании видео по ссылке: {msg.Text} | Метод TTDownloader");
            sr.Close();
        }

        public static void GetErrorLogAboutDeleteBotFromChannel(long userId, string channelName) {
            if (IsTelegramBotOwner(userId)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] У пользователя @{bot.GetChatAsync(userId).Result.Username}({bot.GetChatAsync(userId).Result.Id}) возникла ошибка, скорее всего бот был удалён из канала : {channelName}");
            sr.Close();
        }
    }
}
