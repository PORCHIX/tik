using System;
using System.Threading.Tasks;
using TelegramBotNS;

namespace TelegramBotProject {
    class Program {
        const string token = "1898783341:AAGMhW4nMMfyHkV6lXx6o1P9ue3vCeDD5K0";
        const string tokenTest = "1904203371:AAFUrT0yoddHe00f6-54EatLJjy2hQPu-HU";
        private static TelegramBot TikTokSender;
        static async Task Main() {
            TikTokSender = new TelegramBot(token);
            TikTokSender.Turn_On();
            Console.WriteLine("Выхода нет");
            await AntiExit();
        }
        private static async Task AntiExit() {
            switch ((Console.ReadKey().Key)) { 
                case ConsoleKey.Escape:
                    await AntiExit();
                    break;
                case ConsoleKey.Backspace:
                    Console.Write(" \b");
                    await AntiExit();
                    break;
                default:
                    Console.Write("\b");
                    await AntiExit();
                    break;
            }
        }
    }
}