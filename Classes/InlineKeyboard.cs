using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace InlineKeyboardNS {
    class InlineKeyboard {
        public static InlineKeyboardMarkup GetKeyboard() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { new InlineKeyboardButton { Text = "Опубликовать", CallbackData = "PostVideo"  } },
                new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "Delete" } }
            }); ;
        }
        public static InlineKeyboardMarkup GetKeyboardWithLink(string videoUrl) {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { new InlineKeyboardButton { Text = "Опубликовать", CallbackData = "PostVideo"  } },
                new[] { new InlineKeyboardButton { Text = "Ссылка на тик-ток", Url = videoUrl } },
                new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "Delete" } }
            }); ;
        }
        public static InlineKeyboardMarkup GetSettings() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { new InlineKeyboardButton { Text = "Добавить/Изменить канал", CallbackData = "AddChannel" } }
                //new[] { new InlineKeyboardButton { Text = "Список доступных каналов", CallbackData = "GetChannelList" } }
            }); ;
        }
    }
}

