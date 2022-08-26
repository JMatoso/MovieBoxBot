﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

#nullable disable

namespace MovieBoxBot.Models
{
    internal class TextMessage
    {
        public ChatId ChatId { get; set; }
        public string Text { get; set; }
        public ParseMode ParseMode { get; set; }
        public bool DisableNotifications { get; set; }
        public MessageId ReplyToMessageId { get; set; }
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        public TextMessage(ChatId chatId)
        {
            ChatId = chatId;
            DisableNotifications = false;
        }

        public TextMessage(ChatId chatId, string text)
        {
            ChatId = chatId;
            Text = text;
            DisableNotifications = false;
        }
    }
}