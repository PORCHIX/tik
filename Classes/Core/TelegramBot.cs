using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using InlineKeyboardNS;
using System.Collections.Generic;
using System.Linq;
using TelegramBotNS.Handlers.CallbackHandler;
using TelegramBotNS.Handlers.MessageHandler;
using System.Threading;

namespace TelegramBotNS {
    class TelegramBot {
        public static TelegramBotClient bot;
        public static List<BotCommand> commands;
        public static readonly long telegramBotOwnerId = 1474684994;
        public TelegramBot(string token) {
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
