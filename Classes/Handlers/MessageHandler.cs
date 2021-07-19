using InlineKeyboardNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramBotNS;
using SendersNS;
using DataBaseNS;

namespace TelegramBotProject.Classes.Handlers {
    class MessageHandler {
        private static TelegramBotClient bot = TelegramBot.bot;
        private static List<BotCommand> commands = TelegramBot.commands;
        private static int standartDelay = TelegramBot.standartDelay;
        public static async void OnMessageHandler(object sender, MessageEventArgs e) {
            var msg = e.Message;
            var userId = msg.Chat.Id;
            var filePath = $"../../../Users/{userId}.txt";
            if (!System.IO.File.Exists(filePath)) { System.IO.File.Create(filePath); }

            string TextToSend;
            switch (msg.Text) {
                case "/start":
                    break;
                case "/commands":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    TextToSend =
                        $"Список доступных комманд:\n" +
                        $"/{commands[0].Command} - {commands[0].Description}\n" +
                        $"/{commands[1].Command} - {commands[1].Description}\n" +
                        $"/{commands[2].Command} - {commands[2].Description}";
                    await Sender.SendAutoDeleteMessage(userId, TextToSend, standartDelay);
                    break;
                case "/help":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    TextToSend =
                        "1.Добавьте бота, как администратора в ваш телеграмм канал\n" +
                        $"2.Введите команду /{commands[2].Command}, чтобы настроить бота\n" +
                        "3.Просто скидывайте ссылку на тик ток и бот создаст пост для публикации видео\n\n"+
                        "*** Во время добавления новых каналов - все сообщения воспринимаются ботом, как ссылки на каналы";
                    await Sender.SendAutoDeleteMessage(msg.Chat.Id, TextToSend, standartDelay);
                    break;
                case "/settings":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Найтройки:", replyMarkup: await InlineKeyboard.GetSettings());
                    break;
                default:
                    if (await DataBase.IsUserAddingChannel(userId))
                        await AddChannelHandler(msg);
                    else {
                        if (DataBase.GetUserChannelList(userId).Result.Count != 0) {
                            if (await MessageIsTikTokLink(msg)) { Sender.CreateVideoPost(msg); } 
                            else {
                                await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                                await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Неверная ссылка", standartDelay);
                            }
                        } else { await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Вам не куда постить тик токи, добавьте канаалы,\n воспользовавшись коммандой /{commands[2].Command}", standartDelay); }
                    }
                    break;
            }
        }


        private static async Task AddChannelHandler(Message msg) {
            var userId = msg.Chat.Id;
            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);

            if (await LinkIsCorrect(msg)) {
                if (!await AddableChannelIsAdded(msg)) {
                    if (await BotIsAdmin(msg)) {
                        await AddChannelToUserFile(msg);
                        await Sender.SendAutoDeleteMessage(userId, "Канал успешно добавлен", standartDelay);
                    } else { await Sender.SendAutoDeleteMessage(userId, "Бот не добавлен в этот канал, попробуйте другого или нажмите кнопку: \"Отмена\"", standartDelay); }
                } else { await Sender.SendAutoDeleteMessage(userId, "Канал уже добавлен, попробуйте другой или нажмите кнопку: \"Отмена\"", standartDelay); }
            } else { await Sender.SendAutoDeleteMessage(userId, "Неверная ссылка, попробуйте другую или нажмите кнопку: \"Отмена\"", standartDelay); }

        }


        private static async Task<bool> LinkIsCorrect(Message msg) {
            return (msg.Text.StartsWith("t.me/") || msg.Text.StartsWith("https://t.me/")) && (msg.Text.Length <= 32 + 13);
        }
        private static async Task<string> LinkToChannelName(string link) {
            return $"@{link.Substring(link.IndexOf("t.me/") + 5)}";
        }
        private static async Task<bool> AddableChannelIsAdded(Message msg) {
            long userId = msg.Chat.Id;
            string channelName = await LinkToChannelName(msg.Text);
            string filePath = $"../../../Users/{userId}.txt";
            string[] buffer = await System.IO.File.ReadAllLinesAsync(filePath);
            return Array.Exists(buffer, el => el == channelName);
        }
        private static async Task AddChannelToUserFile(Message msg) {
            long userId = msg.Chat.Id;
            string channelName = await LinkToChannelName(msg.Text);
            string filePath = $"../../../Users/{userId}.txt";
            using (StreamWriter sw = new(filePath, true)) {
                sw.WriteLine(channelName);
            }
        }
        private static async Task<bool> BotIsAdmin(Message msg) {
            bool botIsAdmin = false;
            string channelName = await LinkToChannelName(msg.Text);
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
        private static async Task<bool> MessageIsTikTokLink(Message msg) {
            return (msg.Text.IndexOf("https://www.tiktok.com/") == 0 || msg.Text.IndexOf("https://vm.tiktok.com/") == 0);
        }
    }
}