using System;
using System.IO;
using Telegram.Bot.Args;
using static TelegramBotNS.TelegramBot;
using Telegram.Bot.Types;

namespace LogsNS {
    class Logs {
        private const string logsFilePath = "../../../Logs/logs.txt";
        private static bool IsTelegramBotOwner(long userId) { return telegramBotOwnerId == userId; }
        public static void GetErrorLogAboutPostToChannel (CallbackQueryEventArgs e, string channel) {
            if (IsTelegramBotOwner(e.CallbackQuery.From.Id)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] @{e.CallbackQuery.From.Username}({e.CallbackQuery.From.Id}) - Не смог запостить видео в @{bot.GetChatAsync($"@{channel}").Result.Username}({bot.GetChatAsync($"@{channel}").Result.Id})");
            sr.Close();
        }

        public static void GetErrorLogAboutTTDownloader(long userId,string videoUrl) {
            if (IsTelegramBotOwner(userId)) { return; }
            StreamWriter sr = new(logsFilePath, true);
            sr.WriteLine($"[{DateTime.Now}] Dозникла ошибка при скачивании видео по ссылке: {videoUrl} | Метод TTDownloader");
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
