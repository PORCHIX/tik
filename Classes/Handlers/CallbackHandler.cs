
using DataBaseNS;
using InlineKeyboardNS;
using System.Collections.Generic;
using Telegram.Bot.Args;
using SendersNS;
using LogsNS;
using static TelegramBotNS.TelegramBot;
using System;

namespace TelegramBotNS.Handlers.CallbackHandler {
    class CallbackHandler {
        public static async void OnCallbackHandler(object sender, CallbackQueryEventArgs e) {
            var msg = e.CallbackQuery.Message;
            var userId = msg.Chat.Id;
            List<string> channelList;
            switch (e.CallbackQuery.Data) {
                case "DeleteMessage":
                    await bot.DeleteMessageAsync(chatId: userId, messageId: msg.MessageId);
                    break;
                case "GetAddingingChannelsKeyboard":
                    await Sender.EditTextMessageAsync(userId, msg.MessageId, "Введите ссылку на канал который вы хотите добавить", InlineKeyboard.GetAddingingChannelsKeyboard());
                    await DataBase.AddToUserFile_AddingChanel_Status(userId);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "GetHelpWithFindingChannelLink":
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Ссылка находится во вкладке описание канала, она имеет вид: t.me/yourchannellink.\nПросто кликнув на нёё, вы скопируете её в буфер обмена, далее просто отправьте эту ссылку боту", showAlert: true);
                    break;
                case "CancelAddingChannelsEvent":
                    await DataBase.RemoveFromUserFile_AddingChannel_Status(userId);
                    goto case "ReturnToBotSettingsKeyboard";
                case "GetChannelListKeyboard":
                    channelList = await DataBase.GetUserChannelList(userId);
                    if (!await DataBase.UserChannelListIsEmpty(userId)) {
                        await Sender.EditTextMessageAsync(userId, msg.MessageId, "Список добавленных каналов:", InlineKeyboard.GetChannelListKeyboard(channelList));
                        await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    } else { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"У пользователя: @{msg.Chat.Username} нет добавленных каналов", showAlert: true); }
                    break;
                case "ReturnToBotSettingsKeyboard":
                    await Sender.EditTextMessageAsync(userId, msg.MessageId, "Настройки:", InlineKeyboard.GetBotSettingsKeyboard());
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "ReturnToChannelListKeyboard":
                    goto case "GetChannelListKeyboard";
                case "ApprovePublicationOffer":
                    await bot.SendVideoAsync(chatId: "@zxc_memes", video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                    break;
                case "GetPublicationDesignSettings":
                    await Sender.EditTextMessageAsync(userId, msg.MessageId, "Внесите изменения в оформление публикации", InlineKeyboard.GetPublicationDesignSettings());
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;
                case "InDeveloping":
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id,text: "В разработке", showAlert: true);
                    break;
                default:
                    channelList = await DataBase.GetUserChannelList(userId);
                    foreach (var channel in channelList) {
                        if (e.CallbackQuery.Data == $"PostVideoTo{channel}") {
                            if (await Sender.CanVideoBePostedToChannel(userId, channel)) {
                                if (channel == "zxc_memes" && userId != 474684994) {
                                    await bot.SendVideoAsync(chatId: "@zxc_memes_offers", video: e.CallbackQuery.Message.Video.FileId, caption: $"Предложение на публикацию от: @{bot.GetChatAsync(userId).Result.Username} в ZXC Тик Таки", replyMarkup: InlineKeyboard.GetPublicationOfferKeyboard());
                                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Видео предложено на публикацию", showAlert: true);
                                } 
                                else {
                                    await bot.SendVideoAsync(chatId: $"@{channel}", video: e.CallbackQuery.Message.Video.FileId, disableNotification: true);
                                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: "Видео опубликовано", showAlert: true);
                                }
                            } else { await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id, text: $"Вы не можете запостить видео, т.к. вы и/или бот не являетесь администратором канала @{channel}.", showAlert: true); }
                        }
                        if (e.CallbackQuery.Data == $"Select{channel}ToDeleteFromChannelList") {
                            await Sender.EditTextMessageAsync(userId, msg.MessageId, $"@{channel}", InlineKeyboard.GetDeletingChannelKeyboard(channel));
                            await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                        }
                        if (e.CallbackQuery.Data == $"Delete{channel}FromChannelList") {
                            await DataBase.RemoveChannelFromUserFile(userId, channel);
                            if (!await DataBase.UserChannelListIsEmpty(userId)) { goto case "ReturnToChannelListKeyboard"; } else { goto case "ReturnToBotSettingsKeyboard"; }
                        }
                    }
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id);
                    break;            
            }  
        }
    }
}
