using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

using TTDownloaderNS;
using InlineKeyboardNS;

using System.Collections.Generic;
using System.Linq;
using DataBaseNS;
using System.Threading;
using TelegramBotProject.Classes.Handlers;

namespace TelegramBotNS {
    class TelegramBot {
        public string token;
        public static TelegramBotClient bot;
        private static readonly long myId = 474684994;
        public const string videoPath = @"tiktok.mp4";
        public static List<BotCommand> commands;
        public static readonly int standartDelay = 8000;
        public TelegramBot(string token) {
            this.token = token;
            bot = new TelegramBotClient(token);
            bot.SetMyCommandsAsync(new[] {
                new BotCommand { Command = "commands", Description = "Вывести список всех комманд"},
                new BotCommand { Command = "help", Description = "Получите иинструкцию по настройке бота" },
                new BotCommand { Command = "settings", Description = "Найтройка бота под себя" },
                });
            commands = bot.GetMyCommandsAsync().Result.ToList();
            bot.OnMessage += MessageHandler.OnMessageHandler;
            bot.OnCallbackQuery += CallbackHandler.OnCallbackHandler;
        }
        public void Turn_On() {
            bot.StartReceiving();
        }
        public void Turn_Off() {
            bot.StopReceiving();
        }
    }
}
