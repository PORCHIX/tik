using SendersNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using LogsNS;
using static TelegramBotNS.TelegramBot;
namespace DataBaseNS {
    class DataBase {
        public static string Get_UserDB_filePath(long userId) {
            return $"../../../UsersDB/{userId}.txt";
        }
        public static async Task<List<string>> GetUserChannelList(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            StreamReader sr = new(filePath);
            var fileContent = await sr.ReadToEndAsync();
            sr.Close();
            var channelList = fileContent.Split('@', StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < channelList.Count; i++) {
                channelList[i] = channelList[i].Trim(new[] { '\n', '\r' });
            }
            return channelList;
        }
        public static async Task<bool> UserChannelListIsEmpty(long userId) {
            var channelList = await GetUserChannelList(userId);
            return channelList.Count == 0;
        }
        public static async Task<bool> IsUserAddingChannel(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            string[] buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            return Array.Exists(buffer, el => el.Contains("Status: adding channel"));
        }
        public static async Task AddToUserFile_AddingChanel_Status(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            StreamWriter sw = new(filePath, true);
            await sw.WriteLineAsync("Status: adding channel");
            sw.Close();
            
        }
        public static async Task RemoveFromUserFile_AddingChannel_Status(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            StreamWriter sw = new(filePath);
            foreach (var item in buffer) {
                if (item != "Status: adding channel") { await sw.WriteLineAsync(item); }
            }
            sw.Close();
        }
        public static async Task RemoveChannelFromUserFile(long userId, string channel) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            StreamWriter sw = new(filePath);
            foreach (var item in buffer) {
                if (item != $"@{channel}")
                    await sw.WriteLineAsync(item);
            }
            sw.Close();
        }
        public static async Task AddChannelEvent (Message msg) {
            var userId = msg.Chat.Id;
            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
            if (ChannelLinkIsCorrect(msg)) {
                if (!await AddableChannelIsAdded(msg)) {
                    if (await BotIsAdmin(msg)) {
                        AddChannelToUserFile(msg);
                        await Sender.SendAutoDeleteMessage(userId, "Канал успешно добавлен");
                    } else { await Sender.SendAutoDeleteMessage(userId, "Бот не добавлен в этот канал, попробуйте другого или нажмите кнопку: \"Отмена\""); }
                } else { await Sender.SendAutoDeleteMessage(userId, "Канал уже добавлен, попробуйте другой или нажмите кнопку: \"Отмена\""); }
            } else { await Sender.SendAutoDeleteMessage(userId, "Неверная ссылка, попробуйте другую или нажмите кнопку: \"Отмена\""); }
        }
        private static void AddChannelToUserFile(Message msg) {
            long userId = msg.Chat.Id;
            string channelName = LinkToChannelName(msg.Text);
            string filePath = Get_UserDB_filePath(userId);
            StreamWriter sw = new(filePath, true);
            sw.WriteLine(channelName);
            sw.Close();
        }
        private static bool ChannelLinkIsCorrect(Message msg) {
            return (msg.Text.StartsWith("t.me/") || msg.Text.StartsWith("https://t.me/")) && (msg.Text.Length <= 32 + 13);
        }
        private static async Task<bool> AddableChannelIsAdded(Message msg) {
            long userId = msg.Chat.Id;
            string channelName = LinkToChannelName(msg.Text);
            string filePath = Get_UserDB_filePath(userId);
            string[] channelList = await System.IO.File.ReadAllLinesAsync(filePath);
            return Array.Exists(channelList, channel => channel == channelName);
        }
        private static async Task<bool> BotIsAdmin(Message msg) {
            bool botIsAdmin = false;
            string channelName = LinkToChannelName(msg.Text);
            try {
                var adminList = await bot.GetChatAdministratorsAsync(chatId: channelName);
                foreach (var admin in adminList) {
                    if (admin.User.Id == bot.BotId) {
                        botIsAdmin = true;
                    }
                }
            } catch { return false; }
            return botIsAdmin;
        }
        private static string LinkToChannelName(string link) {
            return $"@{link.Substring(link.IndexOf("t.me/") + 5)}";
        }
    }
}
