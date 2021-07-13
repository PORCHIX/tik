using System;
using System.Threading.Tasks;
using MyTgBot;

namespace TelegramBotForParsingTikTok {
    class Program {
        private static readonly string token = "1785854704:AAE6fqsH9BtOGrUZ92bGhdxr_UCllf0OFes";
        static async Task Main(string[] args) {
            TelegramBot myBot = new TelegramBot(token);
            await myBot.Turn_On();
            if (Console.ReadKey().Key != null) { Console.Write("\b"); await myBot.Turn_Off(); }
        } 
    }
}