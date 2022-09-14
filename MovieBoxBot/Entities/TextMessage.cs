using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MovieBoxBot.Models;

internal class TextMessage
{
    public string Text { get; set; } = default!; 
    public ParseMode ParseMode { get; set; }
    public bool DisableNotifications { get; set; }
    public MessageId? ReplyToMessageId { get; set; }
    public InlineKeyboardMarkup? ReplyMarkup { get; set; }

    public TextMessage()
    {
        DisableNotifications = false;
    }
}
