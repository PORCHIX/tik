using System;
using System.IO;
using Telegram.Bot.Args;
using static TelegramBotNS.TelegramBot;
using Telegram.Bot.Types;
using System.Threading.Tasks;

namespace LogsNS {
    class Logs {
        private const string logsFilePath = "../../../Logs/logs.txt";
        public static async Task GetErrorLogAboutPostToChannelAsync (CallbackQueryEventArgs e, string channel) {
            StreamWriter sw = new(logsFilePath, true);
            await sw.WriteLineAsync($"[{DateTime.Now}] @{e.CallbackQuery.From.Username}({e.CallbackQuery.From.Id}) - Не смог запостить видео в @{bot.GetChatAsync($"@{channel}").Result.Username}({bot.GetChatAsync($"@{channel}").Result.Id})");
            await sw.DisposeAsync();
        }

        public static async Task GetErrorLogAboutTTDownloaderAsync(string videoUrl) {
            StreamWriter sw = new(logsFilePath, true);
            await sw.WriteLineAsync($"[{DateTime.Now}] Dозникла ошибка при скачивании видео по ссылке: {videoUrl} | Метод TTDownloader");
            await sw.DisposeAsync();
        }

        public static async Task GetErrorLogAboutDeleteBotFromChannelAsync(long userId, string channelName) {
            StreamWriter sw = new(logsFilePath, true);
            await sw.WriteLineAsync($"[{DateTime.Now}] У пользователя @{bot.GetChatAsync(userId).Result.Username}({bot.GetChatAsync(userId).Result.Id}) возникла ошибка, скорее всего бот был удалён из канала : {channelName}");
            await sw.DisposeAsync();
        }
    }
}
