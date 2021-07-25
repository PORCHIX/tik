using InlineKeyboardNS;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using SendersNS;
using DataBaseNS;
using static TelegramBotNS.TelegramBot;
using System.Threading.Tasks;

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
                        $"/{commands[2].Command} - {commands[2].Description}\n" +
                        $"/{commands[3].Command} - {commands[3].Description}";
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
                    await SettingsMsgHandler(msg);
                    
                    break;
                case "/donate":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    await Sender.SendTextMessageAsync(msg.Chat.Id, "Помоги проекту:", InlineKeyboard.GetDonationKeyboard());
                    break;
                default:
                    if (await DataBase.IsUserAddingChannel(userId)) { await DataBase.AddingChannelEvent(msg); }else
                    if (!await DataBase.UserChannelListIsEmpty(userId)) {
                        if (MessageIsTikTokLink(msg)) {
                            await Sender.CreateVideoPost(msg);
                        } else {
                            await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                            await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Неверная ссылка");
                        }
                    } else { await Sender.SendAutoDeleteMessage(msg.Chat.Id, $"Вам не куда постить тик токи, добавьте канаалы,\n воспользовавшись коммандой /{commands[2].Command}"); }

                    break;
            }
        }
        private static bool MessageIsTikTokLink(Message msg) {
            return (LinkCorrector(msg.Text).StartsWith("https://www.tiktok.com/") || LinkCorrector(msg.Text).StartsWith("https://vm.tiktok.com/"));
        }
        private static string LinkCorrector(string link) {
            link = link.Trim();
            if (link.Contains("vm.tiktok.com")) {
                if (!link.StartsWith("https://")) { return $"https://{ link.Substring(link.IndexOf("vm.tiktok.com"))}"; }
            }
            if (link.Contains("www.tiktok.com")) {
                if (!link.StartsWith("https://")) { link = $"https://{ link.Substring(link.IndexOf("www.tiktok.com"))}"; }
                if (link.Contains("?")) { return link.Substring(0, link.IndexOf("?")); }
            }
            return link;
        }
        private static async Task SettingsMsgHandler(Message msg) {
            var userId = msg.Chat.Id;
            bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
            if(DataBase.IsUserHaveSettingsMsg(userId))bot.DeleteMessageAsync(chatId: userId, messageId: DataBase.GetLastSettingsMsgId(userId));
            DataBase.RemoveFromUserFile_LastSettingsMsgId(userId);
            DataBase.RemoveFromUserFile_AddingChannel_Status(userId);
            int settingsMsgId = Sender.SendTextMessageReturningMsgId(userId, "Найтройки:", InlineKeyboard.GetBotSettingsKeyboard());
            DataBase.SetLastSettingsMsgId(userId, settingsMsgId);
        }
    }
}