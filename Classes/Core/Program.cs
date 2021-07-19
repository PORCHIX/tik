using System;
using System.Threading.Tasks;
using TelegramBotNS;

namespace TelegramBotProject {
    class Program {
        private static readonly string token = "1898783341:AAGMhW4nMMfyHkV6lXx6o1P9ue3vCeDD5K0";
        public static TelegramBot TikTokSender;
        static void Main() {
            TelegramBot TikTokSender = new (token);
            TikTokSender.Turn_On();
            if (Console.ReadKey().Key != null) 
                Console.Write("\b"); TikTokSender.Turn_Off();
        } 
    }
}