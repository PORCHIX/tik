using System;
using System.Threading.Tasks;
using TelegramBotNS;
using TTDownloaderNS;

namespace TelegramBotProject {
    class Program {
        const string ttSender_token = "1898783341:AAGMhW4nMMfyHkV6lXx6o1P9ue3vCeDD5K0";
        const string ttSender_debug_token = "1904203371:AAFUrT0yoddHe00f6-54EatLJjy2hQPu-HU";
        public static readonly long telegramBotOwnerId = 474684994;
        private static TelegramBot TikTokSender;
        static async Task Main() {
            Console.Title = "TikTokSender";
            TikTokSender = new TelegramBot(ttSender_token);
            TikTokSender.Turn_On();
            ConsoleCommands();
        }


        private static void ConsoleCommands() {
            Console.Clear();
            Console.WriteLine($"Write \"Exit\" to close console app");
            Console.SetCursorPosition(0, 2);
            switch (Console.ReadLine().ToLower()) { 
                case "exit":
                    TikTokSender.Turn_Off();
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleCommands();
                    break;
            }
        }
    }
}