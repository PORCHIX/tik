using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading.Tasks;
using System.IO;
using TikTokDownloader;
using MyInlineKeyboard;
using Telegram.Bot.Types;

namespace MyTgBot {
    class TelegramBot {
        public string token;
        private static TelegramBotClient bot;
        private const long tgAdmin = 474684994;
        private const long tgChannel = -1001528773743;
        private const long tgTestChannel = -1001370346032;
        private const string videoPath = @"C:/tiktok.mp4";
        private static string videoUrl;
        public TelegramBot(string token) {
            this.token = token;
            bot = new TelegramBotClient(token);
            bot.OnMessage += OnMessageHandler;
            bot.OnCallbackQuery += OnCallbackHandler;
            
        }
        private static async void OnMessageHandler(object sender, MessageEventArgs e) {
            var msg = e.Message;
            videoUrl = msg.Text;
            Console.WriteLine("Video link: " + videoUrl);
            //await bot.DeleteMessageAsync(chatId: tgAdmin, messageId: msg.MessageId);
            switch (msg.From.Id){
                case tgAdmin:
                    CreateVideoPost(msg);
                    break;
                default:
                    var infoMsg = await SendTextMessage(msg.From.Id, "Sorry, but this bot is available only for personal use for now");
                    break;
            }

        }
        private static async Task CreateVideoPost(Message msg) {
            if (msg.Video != null) {
                await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                await bot.SendVideoAsync(chatId: msg.From.Id, video: msg.Video.FileId, replyMarkup: UnderBotMessageKeyboard.GetKeyboard()); 
            }
            else if (msg.Text.IndexOf("https://www.tiktok.com/") == 0 || msg.Text.IndexOf("https://vm.tiktok.com/") == 0) {
                try {
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    await TTDownloader.Download(msg.Text, videoPath);
                    await SendVideoFromServer(msg.Chat.Id, videoPath);
                } 
                catch { await SendAutoDeleteMessage(msg.Chat.Id, "Error: Can't Download Video. Most likely the link is incorrect\n\n");}
            } 
            else {
                await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                await SendAutoDeleteMessage(msg.Chat.Id, "Error: Invalid TikTok link, please try again!\n\n");
            }
        }
        private static async void OnCallbackHandler (object sender, CallbackQueryEventArgs e) {
            var msg = e.CallbackQuery.Message;
            
            switch (e.CallbackQuery.Data) {
                case "PostVideo": 
                    Console.WriteLine("Video Posted to " + "\""+ bot.GetChatAsync(tgChannel).Result.Title+ "\"");
                    await bot.SendVideoAsync(chatId: tgChannel, video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                    break;
                case "PostVideo_Test":
                    Console.WriteLine("Video Posted to " + "\"" + bot.GetChatAsync(tgTestChannel).Result.Title + "\"");
                    var testVideo = await bot.SendVideoAsync(chatId: tgTestChannel, video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Video Posted", showAlert:true );
                    break;
                case "FilePath":
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Path: " + videoPath, showAlert: true);
                    break;
                case "Delete":
                    await bot.DeleteMessageAsync(chatId: msg.Chat.Id, messageId: msg.MessageId);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
            }
            //await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
        }


        public async Task Turn_On() {
            ==bot.StartReceiving();
        }
        public async Task Turn_Off() {
            bot.StopReceiving();
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
        private static async Task SendVideoFromServer (long id, string videoPath) {
            FileStream fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);
            await bot.SendVideoAsync(chatId: id, video: fileStream, replyMarkup: UnderBotMessageKeyboard.GetKeyboardWithLink(videoUrl));
            fileStream.Close();
            System.IO.File.Delete(videoPath);
        } 
    }
}
