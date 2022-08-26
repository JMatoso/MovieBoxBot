using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

#nullable disable

namespace MovieBoxBot.Models
{
    internal class PhotoMessage
    {
        public string MovieId { get; set;  }
        public InputOnlineFile Photo { get; set; }
        public string Caption { get; set; }
        public ParseMode ParseMode { get; set; }

        public PhotoMessage()
        {
            ParseMode = ParseMode.Html;
        }
    }
}
