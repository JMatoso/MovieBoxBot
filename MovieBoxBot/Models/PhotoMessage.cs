using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace MovieBoxBot.Models;

internal class PhotoMessage
{
    public string MovieId { get; set; } = default!;
    public InputOnlineFile? Photo { get; set; }
    public string Caption { get; set; } = default!;
    public ParseMode ParseMode { get; set; }

    public PhotoMessage()
    {
        ParseMode = ParseMode.Html;
    }
}
