using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

using TTDownloaderNS;
using InlineKeyboardNS;

using System.Collections.Generic;

namespace TelegramBotNS {
    class TelegramBot {
        public string token;
        private static TelegramBotClient bot;

        private static long tgAdmin = 474684994;
        private static bool channel_is_chosen = false; 
        private const string videoPath = @"C:/tiktok.mp4";
        private static string channelName;
        private static string videoUrl;
        private static string tgСhannel;
        private static BotCommand[] commands;
        private static Message callbackMessage;
        //private static Database userDataBase;
        public TelegramBot(string token) {
            this.token = token;
            bot = new TelegramBotClient(token);
            bot.SetMyCommandsAsync(new[] { new BotCommand { Command = "setchannel", Description = "Найтройка бота под себя" },
                                           new BotCommand { Command = "help", Description = "Получите иинструкцию по настройке бота" }});
            commands = bot.GetMyCommandsAsync().Result;
            

                bot.OnMessage += OnMessageHandler;
            bot.OnCallbackQuery += OnCallbackHandler;
            
        }
        private static async void OnMessageHandler(object sender, MessageEventArgs e) {
            var msg = e.Message;
            if (msg.From.Id == tgAdmin) {
                switch (msg.Text) {
                    case "/start":
                        bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: $"Список доступных комманд:\n/{commands[1].Command} - {commands[1].Description}\n/{commands[0].Command} - {commands[0].Description}");
                        break;
                    case "/help":
                        await bot.SendTextMessageAsync(chatId: msg.Chat.Id, 
                            text: 
                            $"1.Добавьте бота, как администратора в ваш телеграмм канал\n" +
                            $"2.Введите команду /{commands[0].Command} и после отправьте ссылку на чат в который бот будет репостить видео\n" +
                            $"3.Просто скидывайте ссылку на тик ток и бот сам отошлёт её в телеграмм канал");
                        break;
                    case "/setchannel":
                        await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Меню настройки бота:", replyMarkup: InlineKeyboard.GetSettings());
                        break;
                    default:
                        try {
                            using (StreamReader sr = new StreamReader($"../../../Users/{msg.From.Id}.txt")) {
                                channelName = sr.ReadToEnd();
                                Console.WriteLine(channelName);
                                channel_is_chosen = true;
                            }
                        } catch{ }
                        
                            if (channel_is_chosen) {
                                videoUrl = msg.Text;
                                CreateVideoPost(msg);
                        } else {
                            if (callbackMessage == null) {
                                await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                                await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: $"Функции бота недоступны. Выберете канал, воспользовавшись командой /{commands[0].Command} ");
                            }
                        }
                        break;
                }

                if (callbackMessage != null && msg.MessageId == callbackMessage.MessageId + 1) {
                    try {
                        await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                        if (msg.Text.Contains("t.me/")) {
                            channelName = "@" + msg.Text.Substring(msg.Text.IndexOf("t.me/") + 5);
                            var adminList = await bot.GetChatAdministratorsAsync(chatId: channelName);
                            foreach (var admin in adminList) {
                                if (admin.User.Id == bot.BotId) {
                                    await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Канал подключен, теперь вы можете скидывать сюда ссылки на тик ток видео");
                                    channel_is_chosen = true;
                                }
                            }
                            callbackMessage = null;
                            using (StreamWriter sw = new StreamWriter($"../../../Users/{msg.From.Id}.txt")) {
                                sw.WriteLine(channelName);
                            }
                        }
                    } catch { await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: $"Бот должен быть добавлен в:\n{msg.Text}"); }
                }
            } else { await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Бот пока только для личного использования"); }
            
            
            
                    
                
            
            
        }
        private static async Task CreateVideoPost(Message msg) {
            if (msg.Video != null) {
                await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                await bot.SendVideoAsync(chatId: msg.From.Id, video: msg.Video.FileId, replyMarkup: InlineKeyboard.GetKeyboard());
            } else if (msg.Text.IndexOf("https://www.tiktok.com/") == 0 || msg.Text.IndexOf("https://vm.tiktok.com/") == 0) {
                try {
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    var uploadingMsg = await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Видео загружается...");
                    await TTDownloader.Download(msg.Text, videoPath);
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: uploadingMsg.MessageId);
                    await SendVideoFromServer(msg.Chat.Id, videoPath);
                } catch { await SendAutoDeleteMessage(msg.Chat.Id, "Error: Can't Download Video. Most likely the link is incorrect\n\n"); }
            } else {
                await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                await SendAutoDeleteMessage(msg.Chat.Id, "Error: Invalid link, please try again!\n\n");
            }
        }
        private static async void OnCallbackHandler(object sender, CallbackQueryEventArgs e) {
            var msg = e.CallbackQuery.Message;

            switch (e.CallbackQuery.Data) {
                case "PostVideo":
                    try {
                        Console.WriteLine("Video Posted to " + "\"" + channelName + "\"");
                        await bot.SendVideoAsync(chatId: channelName, video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                        await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Video Posted", showAlert: true);
                    } catch { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Возможно вы не дали боту права отправлять сообщения", showAlert: true); }
                    break;
                case "FilePath":
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Path: " + videoPath, showAlert: true);
                    break;
                case "Delete":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "AddChannel":
                        try {
                        channel_is_chosen = false;
                        callbackMessage = await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Введите ссылку на телеграм канал");
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        } catch {
                            await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Бот не является администратором канала");
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        }
                        break;
            }
        }
        private static async Task<Message> SendTextMessage(long id, string text) {
            Console.WriteLine(text);
            return await bot.SendTextMessageAsync(text: text, chatId: id);
        }
        private static async Task SendAutoDeleteMessage(long id, string text) {
            var autodeleteMsg = await SendTextMessage(id, text);
            await Task.Delay(5000);
            await bot.DeleteMessageAsync(chatId: id, messageId: autodeleteMsg.MessageId);
        }
        private static async Task SendVideoFromServer(long id, string videoPath) {
            FileStream fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);
            await bot.SendVideoAsync(chatId: id, video: fileStream, replyMarkup: InlineKeyboard.GetKeyboardWithLink(videoUrl));
            fileStream.Close();
            System.IO.File.Delete(videoPath);
        }
        public async Task Turn_On() {
            bot.StartReceiving();
        }
        public async Task Turn_Off() {
            bot.StopReceiving();
        }
    }
}
