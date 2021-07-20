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
using DataBaseNS;
using LogsNS;
using static TelegramBotNS.TelegramBot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SendersNS {
    class Sender {
        public static async Task<Message> SendTextMessage(long userId, string text) {
            return await bot.SendTextMessageAsync(text: text, chatId: userId/*, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html*/);
        }
        public static async Task<Message> SendTextMessage(long userId, string text, InlineKeyboardMarkup keyboard) {
            return await bot.SendTextMessageAsync(text: text, chatId: userId, replyMarkup: keyboard/*, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html*/);
        }
        public static async Task SendAutoDeleteMessage(long userId, string text) {
            var autodelete_msg = await SendTextMessage(userId, text);
            await Task.Delay(5000);
            await bot.DeleteMessageAsync(chatId: userId, messageId: autodelete_msg.MessageId);
        }
        public static async Task SendAutoDeleteMessage(long userId, string text, int delay) {
            var autodelete_msg = await SendTextMessage(userId, text);
            await Task.Delay(delay);
            await bot.DeleteMessageAsync(chatId: userId, messageId: autodelete_msg.MessageId);
        }
        public static async Task<Message> EditTextMessage (long userId, int msgId, string txt, InlineKeyboardMarkup keyboard) {

            return await bot.EditMessageTextAsync(chatId: userId, messageId: msgId, text:$"<strong><i>{txt, -10}</i></strong>" , replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html) ;
        }
        public static async Task CreateVideoPost(Message msg) {
            string videoPath = @$"../../../VideoBuffer/tiktok{msg.Chat.Id}.mp4";
            var videoUrl = msg.Text;
            var userId = msg.Chat.Id;
            var videoIsDownloading_msg = await bot.SendTextMessageAsync(chatId: userId, text: "Видео загружается...");
            try { await TTDownloader.Download(msg.Text, videoPath); } catch {
                await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
                await SendAutoDeleteMessage(userId, "Видео недоступно");
                Logs.GetErrorLogAboutTTDownloader(msg);
                return;
            }
            using (FileStream fileStream = new(videoPath, FileMode.Open, FileAccess.Read)) { await bot.SendVideoAsync(chatId: userId, video: fileStream, replyMarkup: await InlineKeyboard.GetVideoPostKeyboard(userId, videoUrl)); Logs.GetLogAboutTikTokLink(userId,videoUrl); }
            await bot.DeleteMessageAsync(chatId: userId, messageId: videoIsDownloading_msg.MessageId);
            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
            System.IO.File.Delete(videoPath);
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
                    if (admin.User.Id == bot.BotId) {
                        botCanPostMessages = true;
                    }
                }
            } catch { Logs.GetErrorLogAboutDeleteBotFromChannel(userId,channelName); return false; }
            return userCanPostMessages&&botCanPostMessages;
        }
    }
}
