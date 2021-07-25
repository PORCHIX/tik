using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using DataBaseNS;
using System.Threading.Tasks;

namespace InlineKeyboardNS {
    class InlineKeyboard {
        //Публикация
        public static async Task<InlineKeyboardMarkup> GetVideoSendingKeyboard(long userId, string videoUrl) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            var channelList = await DataBase.GetUserChannelList(userId);
            foreach(var channel in channelList) {
                keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = $"Опубликовать в @{channel}", CallbackData = $"PostVideoTo{channel}" } });
            }
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Ссылка на оригинал", Url = videoUrl } });
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Отмена", CallbackData = "DeleteMessage" } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup GetPublicationOfferKeyboard() {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Опубликовать", CallbackData = $"ApprovePublicationOffer" } });
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Отклонить", CallbackData = "DeleteMessage" } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        //Настройки
        public static InlineKeyboardMarkup GetBotSettingsKeyboard() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { 
                    new InlineKeyboardButton { Text = "Добавить канал", CallbackData = "GetAddingingChannelsKeyboard" },
                    new InlineKeyboardButton { Text = "Список каналов", CallbackData = "GetChannelListKeyboard" },
                },
                new[] {new InlineKeyboardButton { Text = "Изменить оформление публикации", CallbackData="GetPublicationDesignSettings" } },
                new[] {new InlineKeyboardButton { Text = "Закрыть", CallbackData = "DeleteMessage" } }
            }); ;
        }
        public static InlineKeyboardMarkup GetAddingingChannelsKeyboard() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() { new[] {
                new InlineKeyboardButton { Text = "Помощь", CallbackData = "GetHelpWithFindingChannelLink" },
                new InlineKeyboardButton { Text = "Отмена", CallbackData = "CancelAddingChannelsEvent" }
                }});
        }
        public static InlineKeyboardMarkup GetChannelListKeyboard(List<string> channelList) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            foreach (var channel in channelList) {
                keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = $"@{channel}", CallbackData = $"Select{channel}ToDeleteFromChannelList" } });
            }
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "ReturnToBotSettingsKeyboard", } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup GetDeletingChannelKeyboard(string channel) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            keyboardButtonList.Add(new[] {
                new InlineKeyboardButton { Text = $"Удалить канал из списка", CallbackData = $"Delete{channel}FromChannelList" },
                new InlineKeyboardButton { Text = "Назад", CallbackData = "ReturnToChannelListKeyboard" }
            }); ; ;
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup GetPublicationDesignSettings() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() { 
                new[] {new InlineKeyboardButton { Text = $"Ссылка на оригинал: выкл", CallbackData = "InDeveloping" } },
                new[] { new InlineKeyboardButton { Text = $"Добавить/Изменить подпись\nк посту", CallbackData = "InDeveloping" } },
                new[] {new InlineKeyboardButton { Text = $"Уведомления при публикации: выкл", CallbackData = "InDeveloping" } },
                new[] {new InlineKeyboardButton { Text = "Отмена", CallbackData = "ReturnToBotSettingsKeyboard" } }
                });
        }
        
        //Донат
        public static InlineKeyboardMarkup GetDonationKeyboard() {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Подпишись на ZXC Тик Таки", Url = "https://t.me/zxc_memes" }, new InlineKeyboardButton { Text = "Закрыть", CallbackData = "DeleteMessage", } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
    }
}

