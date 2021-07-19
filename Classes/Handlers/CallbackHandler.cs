using DataBaseNS;
using InlineKeyboardNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramBotNS;
using SendersNS;
using LogsNS;

namespace TelegramBotProject.Classes.Handlers {
    class CallbackHandler {
        private static TelegramBotClient bot = TelegramBot.bot;
        
        public static async void OnCallbackHandler(object sender, CallbackQueryEventArgs e) {
            var msg = e.CallbackQuery.Message;
            var userId = msg.Chat.Id;
            List<string> channelList;
            switch (e.CallbackQuery.Data) {
                case "CloseSettings":
                    await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                    break;
                case "AddChannel":
                    await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text:"Введите ссылку на канал который вы хотите добавить", replyMarkup: await InlineKeyboard.AddingingChannelKeyboard());
                    await DataBase.AddToUserFile_AddingChanel_Status(userId);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "HelpWithFindLink":
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Ссылка находится во вкладке описание канала, она имеет вид: t.me/yourchannellink. Просто кликнув на нёё, вы скопируете её в буфер обмена, далее просто отправьте эту ссылку боту", showAlert: true);
                    break;
                case "CancelAddingChannel":
                    await DataBase.RemoveFromUserFile_AddingChannel_Status(userId);
                    goto case "BackToSettings";
                case "GetChannelList":
                    channelList = await DataBase.GetUserChannelList(userId);
                    if (channelList.Count != 0) {
                        await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId,text: "Список добавленных каналов:", replyMarkup: await InlineKeyboard.GetChannelList(channelList));
                        await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    } else { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"У пользователя: {msg.Chat.Username} нет добавленных каналов", showAlert: true); }
                    break;
                case "BackToSettings":
                    await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text: "Настройки:", replyMarkup: await InlineKeyboard.GetSettings());
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "BackToChannelList":
                    goto case "GetChannelList";
                case "DeletePost":
                    await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                    break;
                default:
                    channelList = await DataBase.GetUserChannelList(userId);
                    foreach (var channel in channelList) {
                        if (e.CallbackQuery.Data == channel) {
                            await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text: $"{channel}", replyMarkup: await InlineKeyboard.DeleteChannel(channel));
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        }
                        if (e.CallbackQuery.Data == $"Delete_{channel}") {
                            DataBase.RemoveFromFile_ChannelName(userId, channel);
                            if (DataBase.GetUserChannelList(userId).Result.Count != 0) { goto case "BackToChannelList"; } else { goto case "BackToSettings"; }
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        }
                        if (e.CallbackQuery.Data == $"PostVideoTo{channel}") {
                            if (await Sender.UserCanSendVideoToChannel(msg, channel)) {
                                await bot.SendVideoAsync(chatId: $"@{channel}", video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                                await Logs.Getter(e, channel);
                                await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Видео опубликовано", showAlert: true);
                            } else {
                                await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Вы не можете запостить видео, т.к. вы не являетесь администратором канала", showAlert: true);
                            }
                        }
                    }
                    break;             
            }  
        }
    }
}
