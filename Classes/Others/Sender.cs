using InlineKeyboardNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotNS;
using TTDownloaderNS;
using LogsNS;

namespace SendersNS {
    class Sender {
        private static TelegramBotClient bot = TelegramBot.bot;
        public static async Task<Message> SendTextMessage(long id, string text) {
            //Console.WriteLine(text);
            return await bot.SendTextMessageAsync(text: text, chatId: id/*, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html*/);
        }
        public static async Task SendAutoDeleteMessage(long id, string text, int delay) {
            try {
                var autodelete_msg = await SendTextMessage(id, text);
                await Task.Delay(delay);
                await bot.DeleteMessageAsync(chatId: id, messageId: autodelete_msg.MessageId);
            } catch (Exception e) { Console.WriteLine(e); }

        }

        public static async Task CreateVideoPost(Message msg) {

            var standartDelay = TelegramBot.standartDelay;
            string videoPath = TelegramBot.videoPath;
            var videoUrl = msg.Text;
            var userId = msg.Chat.Id;
            var videoIsDownloading_msg = await bot.SendTextMessageAsync(chatId: userId, text: "Видео загружается...");
            try { await TTDownloader.Download(msg.Text, videoPath); } catch {
                await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
                await SendAutoDeleteMessage(userId, "Видео недоступно", standartDelay);
                return;
            }
            using (FileStream fileStream = new(videoPath, FileMode.Open, FileAccess.Read)) { await bot.SendVideoAsync(chatId: userId, video: fileStream, replyMarkup: await InlineKeyboard.GetVideoPostKeyboard(userId, videoUrl)); }
            await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
            System.IO.File.Delete(videoPath);
            Logs.Getter(videoUrl);
        }
        public static async Task<bool> UserCanSendVideoToChannel (Message msg, string channel) {
            bool userCanSendPost = false;
            string channelName = $"@{channel}";
            try {
                var adminList = await bot.GetChatAdministratorsAsync(chatId: channelName);
                foreach (var admin in adminList) {
                    if (admin.User.Id == bot.BotId && (bool)admin.CanPostMessages) {
                        userCanSendPost = true;
                    }
                }
            } catch { return false; }
            return userCanSendPost;
        }
        
    }
}
