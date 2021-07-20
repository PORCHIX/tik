
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
using SendersNS;
using LogsNS;
using static TelegramBotNS.TelegramBot;

namespace TelegramBotNS.Handlers.CallbackHandler {
    class CallbackHandler {
        public static async void OnCallbackHandler(object sender, CallbackQueryEventArgs e) {
            var msg = e.CallbackQuery.Message;
            var userId = msg.Chat.Id;
            List<string> channelList;
            switch (e.CallbackQuery.Data) {
                case "CloseSettings":
                    await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                    break;
                case "AddChannel":
                    //await Sender.EditTextMessage(userId, msg.MessageId, "Введите ссылку на канал который вы хотите добавить", InlineKeyboard.AddingingChannelKeyboard());
                    await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text:"Введите ссылку на канал который вы хотите добавить", replyMarkup: InlineKeyboard.AddingingChannelKeyboard());
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
                    if (!await DataBase.UserChannelListIsEmpty(userId)) {
                        await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId,text: "Список добавленных каналов:", replyMarkup: InlineKeyboard.GetChannelList(channelList));
                        await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    } else { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"У пользователя: @{msg.Chat.Username} нет добавленных каналов", showAlert: true); }
                    break;
                case "BackToSettings":
                    await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text: "Настройки:", replyMarkup: InlineKeyboard.GetSettings());
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "BackToChannelList":
                    goto case "GetChannelList";
                case "DeletePost":
                    await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                    break;
                case "ZXCoffer":
                    await bot.SendVideoAsync(chatId: "@tiktoksender_test", video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                    break;
                default:
                    channelList = await DataBase.GetUserChannelList(userId);
                    foreach (var channel in channelList) {
                        if (e.CallbackQuery.Data == channel) {
                            //await Sender.EditTextMessage(userId, msg.MessageId, $"{channel}", InlineKeyboard.DeleteChannel(channel));
                            await bot.EditMessageTextAsync(chatId: userId, messageId: msg.MessageId, text: $"{channel}", replyMarkup: InlineKeyboard.DeleteChannel(channel));
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        }
                        if (e.CallbackQuery.Data == $"Delete_{channel}") {
                            await DataBase.RemoveChannelFromUserFile(userId, channel);
                            if (!await DataBase.UserChannelListIsEmpty(userId)) { goto case "BackToChannelList"; } else { goto case "BackToSettings"; }
                        }
                        if (e.CallbackQuery.Data == $"PostVideoTo{channel}") {
                            if (await Sender.CanVideoBePostedToChannel(userId, channel)) {
                                try {
                                    if (channel == "tiktoksender_test" && userId != 474684994) {
                                        await bot.SendVideoAsync(chatId: "@zxc_memes_offers", video: e.CallbackQuery.Message.Video.FileId, caption: $"Предложение на публикацию от: {bot.GetChatAsync(userId).Result.Username} в ZXC Тик Таки", replyMarkup: InlineKeyboard.Get_ZXC_Keyboard(), disableNotification: true);
                                        await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Видео предложено на публикацию ZXC Тик Таки", showAlert: true);
                                    } else {
                                        await bot.SendVideoAsync(chatId: $"@{channel}", video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                                        Logs.GetLogAboutPostToChannel(e, channel);
                                    }
                                } catch {
                                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"У бота отключена возможность отправки сообщений в @{channel}, обратитесь к владельцу канала, чтобы это исправить", showAlert: true);
                                    Logs.GetErrorLogAboutPostToChannel(e, channel);
                                }
                                await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Видео опубликовано", showAlert: true);
                            } else { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"Вы не можете запостить видео, т.к. вы или бот не являетесь администратором в @{channel}.", showAlert: true); }
                        }
                    }
                    break;            
            }  
        }
    }
}
