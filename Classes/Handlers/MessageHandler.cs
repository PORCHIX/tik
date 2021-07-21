using InlineKeyboardNS;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using SendersNS;
using DataBaseNS;
using static TelegramBotNS.TelegramBot;

namespace TelegramBotNS.Handlers.MessageHandler {
    class MessageHandler {
        public static async void OnMessageHandler(object sender, MessageEventArgs e) {
            var msg = e.Message;
            var userId = msg.Chat.Id;
            var filePath = $"../../../UsersDB/{userId}.txt";
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
                    await Sender.SendAutoDeleteMessage(userId, TextToSend);
                    break;
                case "/help":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    TextToSend =
                        "1.Добавьте бота, как администратора в ваш телеграмм канал\n" +
                        $"2.Введите команду /{commands[2].Command}, чтобы настроить бота\n" +
                        "3.Просто скидывайте ссылку на тик ток и бот создаст пост для публикации видео\n\n"+
                        "*** Во время добавления новых каналов - все сообщения воспринимаются ботом, как ссылки на каналы";
                    await Sender.SendAutoDeleteMessage(msg.Chat.Id, TextToSend);
                    break;
                case "/settings":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    //await Sender.SendTextMessage(msg.Chat.Id, "Настройки", InlineKeyboard.GetSettings());
                    await bot.SendTextMessageAsync(chatId: msg.Chat.Id, text: "Найтройки:", replyMarkup: InlineKeyboard.GetSettings());
                    break;
                default:
                    if (await DataBase.IsUserAddingChannel(userId)) { await DataBase.AddChannelEvent(msg); } else {
                        if (! await DataBase.UserChannelListIsEmpty(userId)) {
                            if (MessageIsTikTokLink(msg)) {
                                await Sender.CreateVideoPost(msg);
                            } 
                            else {
                                await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                                await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Неверная ссылка");
                            }
                        } else { await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Вам не куда постить тик токи, добавьте канаалы,\n воспользовавшись коммандой /{commands[2].Command}"); }
                    }
                    break;
            }
        }
        private static bool MessageIsTikTokLink(Message msg) {
            return (msg.Text.StartsWith("https://www.tiktok.com/") || msg.Text.StartsWith("https://vm.tiktok.com/"));
        }



    }
}