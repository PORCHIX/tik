using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using DataBaseNS;
using System.Threading.Tasks;

namespace InlineKeyboardNS {
    class InlineKeyboard {
        public static async Task<InlineKeyboardMarkup> GetVideoPostKeyboard(long userId, string videoUrl) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            var channelList = await DataBase.GetUserChannelList(userId);
            foreach(var channel in channelList) {
                keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = $"Отправить в @{channel}", CallbackData = $"PostVideoTo{channel}" } });
            }
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Ссылка на тик-ток", Url = videoUrl } });
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "DeletePost" } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup Get_ZXC_Keyboard() {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = $"Опубликовать в ZXC Тик Таки", CallbackData = $"ZXCoffer" } });
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "DeletePost" } });
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup GetSettings() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                //new[] {new InlineKeyboardButton{ Text = "Задонять пожожда", Pay = true,    } },
                new[] { 
                    new InlineKeyboardButton { Text = "Добавить канал", CallbackData = "AddChannel" },
                    new InlineKeyboardButton { Text = "Список каналов", CallbackData = "GetChannelList" }
                },
                new[] {new InlineKeyboardButton { Text = "Закрыть", CallbackData = "CloseSettings" } }
            }); ;
        }
        public static InlineKeyboardMarkup GetChannelList(List<string> channelList) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            foreach (var channel in channelList) {
                keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = $"@{channel}", CallbackData= channel} }) ;    
            }
            keyboardButtonList.Add(new[] { new InlineKeyboardButton { Text = "Назад", CallbackData = "BackToSettings", } });
            return new InlineKeyboardMarkup(keyboardButtonList) ;
        }
        public static InlineKeyboardMarkup DeleteChannel (string channel) {
            var keyboardButtonList = new List<InlineKeyboardButton[]>();
            keyboardButtonList.Add(new[] {
                new InlineKeyboardButton { Text = $"Удалить {channel}", CallbackData = $"Delete_{channel}" },
                new InlineKeyboardButton { Text = "Назад", CallbackData = "BackToChannelList" }
            }); ; ;
            return new InlineKeyboardMarkup(keyboardButtonList);
        }
        public static InlineKeyboardMarkup AddingingChannelKeyboard() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() { new[] {
                new InlineKeyboardButton { Text = "Помощь", CallbackData = "HelpWithFindLink" },
                new InlineKeyboardButton { Text = "Отмена", CallbackData = "CancelAddingChannel" }
                }});
        }


    }
}

