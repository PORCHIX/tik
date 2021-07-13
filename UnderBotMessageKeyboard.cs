using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyInlineKeyboard {
    class UnderBotMessageKeyboard {
        public static InlineKeyboardMarkup GetKeyboard() {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { new InlineKeyboardButton { Text = "Опубликовать", CallbackData = "PostVideo"  } },
                new[] { new InlineKeyboardButton { Text = "Опубликовать(Test)", CallbackData = "PostVideo_Test"  } },
                new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "Delete" } }
            }); ;
        }
        public static InlineKeyboardMarkup GetKeyboardWithLink(string videoUrl) {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>() {
                new[] { new InlineKeyboardButton { Text = "Опубликовать", CallbackData = "PostVideo"  } },
                new[] { new InlineKeyboardButton { Text = "Опубликовать(Test)", CallbackData = "PostVideo_Test"  } },
                new[] { new InlineKeyboardButton { Text = "Ссылка на тик-ток", Url = videoUrl } },
                new[] { new InlineKeyboardButton { Text = "Удалить", CallbackData = "Delete" } }
            }); ;
        }
    }
}

