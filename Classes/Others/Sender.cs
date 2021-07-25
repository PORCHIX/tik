using InlineKeyboardNS;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TTDownloaderNS;
using LogsNS;
using static TelegramBotNS.TelegramBot;
using Telegram.Bot.Types.ReplyMarkups;
using System;

namespace SendersNS {
    class Sender {
        private static string GeneralTextMessageStyling(string text) { return $"<b>{text}</b>"; }

        public static async Task<Message> SendTextMessageAsync(long userId, string text) {
            return await bot.SendTextMessageAsync(chatId: userId, text: GeneralTextMessageStyling(text), parseMode:Telegram.Bot.Types.Enums.ParseMode.Html );
        }
        public static int SendTextMessageReturningMsgId(long userId, string text, InlineKeyboardMarkup keyboard) {
            var msg = bot.SendTextMessageAsync(chatId: userId, text: GeneralTextMessageStyling(text), replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            return msg.Result.MessageId;
        }
        public static async Task<Message> SendTextMessageAsync(long userId, string text, InlineKeyboardMarkup keyboard) {
            return await bot.SendTextMessageAsync(chatId: userId, text: GeneralTextMessageStyling(text), replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task SendAutoDeleteMessage(long userId, string text) {
            var autodelete_msg = await SendTextMessageAsync(userId, text);
            await Task.Delay(10000);
            await bot.DeleteMessageAsync(chatId: userId, messageId: autodelete_msg.MessageId);
        }
        public static async Task SendAutoDeleteMessage(long userId, string text, int delay) {
            var autodelete_msg = await SendTextMessageAsync(userId, text);
            await Task.Delay(delay);
            await bot.DeleteMessageAsync(chatId: userId, messageId: autodelete_msg.MessageId);
        }
        public static async Task<Message> EditTextMessageAsync (long userId, int msgId, string text, InlineKeyboardMarkup keyboard) {
            return await bot.EditMessageTextAsync(chatId: userId, messageId: msgId, text:GeneralTextMessageStyling(text) , replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html) ;
        }
        
        public static async Task CreateVideoPost(Message msg) {
            string videoPath = @$"../../../VideoBuffer/tiktok{msg.Chat.Id}{new Random().Next()}.mp4";
            var videoUrl = msg.Text;
            var userId = msg.Chat.Id;
            var videoIsDownloading_msg = await SendTextMessageAsync(userId, "Видео загружается...");
            await TTDownloader.Download(msg.Text, videoPath);
            if (System.IO.File.Exists(videoPath)) {
                FileStream fileStream = new(videoPath, FileMode.Open, FileAccess.Read);
                await bot.SendVideoAsync(chatId: userId, video: fileStream, replyMarkup: await InlineKeyboard.GetVideoSendingKeyboard(userId, videoUrl));
                await fileStream.DisposeAsync();
                await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
                await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                System.IO.File.Delete(videoPath);
            } else {
                await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
                await bot.DeleteMessageAsync(chatId: userId, messageId:msg.MessageId);
                await SendAutoDeleteMessage(userId, "Видео недоступно");
                await Logs.GetErrorLogAboutTTDownloaderAsync( videoUrl);
            }
        }
        public static async Task<bool> CanVideoBePostedToChannel (long userId, string channel) {
            bool userCanPostMessages = false;
            bool botCanPostMessages = false;
            string channelName = $"@{channel}";
            try {
                var adminList = await bot.GetChatAdministratorsAsync(chatId: channelName);
                foreach (var admin in adminList) {
                    if (admin.User.Id == userId) {
                        userCanPostMessages = true;
                    }
                    if (admin.User.Id == bot.BotId && (bool)admin.CanPostMessages) {
                        botCanPostMessages = true;
                    }
                }
            } catch { await Logs.GetErrorLogAboutDeleteBotFromChannelAsync(userId, channelName); return false; }
            return userCanPostMessages&&botCanPostMessages;
        }
    }
}
