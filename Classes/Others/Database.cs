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
        public static int GetLastSettingsMsgId(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = System.IO.File.ReadAllLines(filePath);
            foreach (var item in buffer) {
                if (item.Contains("#!LastSettingsMsgId: ")) { return Convert.ToInt32(item.Substring(21)); }
            }
            return 0;
        }
        public static async Task SetLastSettingsMsgId(long userId, long msgId) {
            var filePath = Get_UserDB_filePath(userId);
            StreamWriter sw = new(filePath, true);
            await sw.WriteLineAsync($"#!LastSettingsMsgId: {msgId}");
            sw.Dispose();
        }
        public static async Task RemoveFromUserFile_LastSettingsMsgId (long userId) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            StreamWriter sw = new(filePath);
            foreach (var item in buffer) {
                if (!item.Contains("#!LastSettingsMsgId")) { await sw.WriteLineAsync(item); }
            }
            sw.Dispose();
        }
        public static bool IsUserHaveSettingsMsg (long userId) {
            var filePath = Get_UserDB_filePath(userId);
            string[] buffer = System.IO.File.ReadAllLines(filePath);
            return Array.Exists(buffer, line => line.Contains("#!LastSettingsMsgId:"));
        }
        public static async Task<bool> IsUserAddingChannel(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            string[] buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            return Array.Exists(buffer, line => line.Contains("#!Status: Adding channel"));
        }
        public static async Task AddToUserFile_AddingChanel_Status(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            StreamWriter sw = new(filePath, true);
            await sw.WriteLineAsync("#!Status: Adding channel");
            await sw.DisposeAsync();

        }
        
        public static async Task RemoveFromUserFile_AddingChannel_Status(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            StreamWriter sw = new(filePath);
            foreach (var item in buffer) {
                if (item != "#!Status: Adding channel") { await sw.WriteLineAsync(item); }
            }
            sw.DisposeAsync();
        }
        public static async Task AddingChannelEvent(Message msg) {
            var userId = msg.Chat.Id;
            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
            if (ChannelLinkIsCorrect(msg)) {
                if (!await AddableChannelIsAdded(msg)) {
                    if (await BotIsAdmin(msg)) {
                        await AddChannelToUserFile(msg);
                        await Sender.SendAutoDeleteMessage(userId, "Канал успешно добавлен");
                    } else { await Sender.SendAutoDeleteMessage(userId, "Бот не добавлен в этот канал, попробуйте другого или нажмите кнопку: \"Отмена\""); }
                } else { await Sender.SendAutoDeleteMessage(userId, "Канал уже добавлен, попробуйте другой или нажмите кнопку: \"Отмена\""); }
            } else { await Sender.SendAutoDeleteMessage(userId, "Неверная ссылка, попробуйте другую или нажмите кнопку: \"Отмена\""); }
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
        private static async Task AddChannelToUserFile(Message msg) {
            long userId = msg.Chat.Id;
            string channelName = LinkToChannelName(msg.Text);
            string filePath = Get_UserDB_filePath(userId);
            StreamWriter sw = new(filePath, true);
            await sw.WriteLineAsync(channelName);
            await sw.DisposeAsync();
        }
        public static async Task RemoveChannelFromUserFile(long userId, string channel) {
            var filePath = Get_UserDB_filePath(userId);
            var buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            StreamWriter sw = new(filePath);
            foreach (var item in buffer) {
                if (item != $"@{channel}")
                    await sw.WriteLineAsync(item);
            }
            await sw.DisposeAsync();
        }
        public static async Task<bool> UserChannelListIsEmpty(long userId) {
            var channelList = await GetUserChannelList(userId);
            return channelList.Count == 0;
        }

        public static async Task<List<string>> GetUserChannelList(long userId) {
            var filePath = Get_UserDB_filePath(userId);
            StreamReader sr = new(filePath);
            var fileContent = await sr.ReadToEndAsync();
            sr.Dispose();
            var bufferList = fileContent.Split(new[] { '@','#' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> channelList = new List<string>();
            for (int i = 0; i < bufferList.Count; i++) {
                if (!bufferList[i].Contains('!'))
                    channelList.Add(bufferList[i].Trim(new[] { '\n', '\r' }));
            }
            return channelList;
        }
        private static string LinkToChannelName(string link) {
            return $"@{link.Substring(link.IndexOf("t.me/") + 5)}";
        }
    }
}
