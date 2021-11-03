using System;
using System.Threading.Tasks;
using TelegramBotNS;
using TTDownloaderNS;

namespace TelegramBotProject {
    class Program {
        const string ttSender_token = "2054038321:AAHrWbcc99iRFT2DMZan33hyOzfhCNwQqK4";
        const string ttSender_debug_token = "2054038321:AAHrWbcc99iRFT2DMZan33hyOzfhCNwQqK4";
        public static readonly long telegramBotOwnerId = 1719045064;
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
