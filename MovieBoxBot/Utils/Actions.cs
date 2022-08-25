using MovieBoxBot.Models;
using MovieBoxBot.Utils.Client;
using System.Text.Encodings.Web;
using Telegram.Bot.Types;

namespace MovieBoxBot.Utils
{
    internal class Actions
    {
        private static string baseUrl = "https://yts.mx/api/v2/";
        private static readonly HttpClientService<Root> _httpClientService = new HttpClientService<Root>();

        public static TextMessage Help(ChatId chatId) => new(chatId)
        {
            Text = "I am the <a href =\"http://www.moviebox.site\">MovieBox</a> website bot, I will help you to search, see the details and download torrents of the movies you want." +
                Environment.NewLine + Environment.NewLine + "I was created by <a href =\"http://github.com/JMatoso/MovieBoxBot\">José Matoso</a>, in order to help <a href =\"http://www.moviebox.site\">MovieBox</a> users getting torrents easier.",
            ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
        };

        public static TextMessage Start(ChatId chatId, string name) => new(chatId)
        {
            Text = $"Let's get started, {name}! Tell me what you want to do."
        };

        public static IEnumerable<PhotoMessage> ListLatest(ChatId chatId)
        {
            var result = _httpClientService.GetAsync($"{baseUrl}list_movies.json?limit=4").Result;

            foreach(var movie in result.Data.Movies)
            {
                yield return new PhotoMessage(chatId)
                {
                    Photo = movie.MediumCoverImage,
                    Caption = $"<b>{movie.TitleLong}</b>" + Environment.NewLine + GetStringCollection(movie.Genres)
                        + Environment.NewLine + Environment.NewLine + movie.DescriptionFull + Environment.NewLine
                        + Environment.NewLine + GetTorrents(movie.Torrents, movie.Id.ToString(), movie.Title)
                };
            }
        }

        private static string GetTorrents(List<Torrent> torrents, string movieId, string movieTitle)
        {
            var list = string.Empty;

            foreach(var torrent in torrents)
            {
                list += $"<a href=\"{torrent.Url}\">{torrent.Quality} - {torrent.Size}</a>" + Environment.NewLine;
            }

            return list;
        }

        private static string GetStringCollection(List<string> list)
        {
            var newValue = string.Empty;

            foreach(var text in list)
            {
                newValue += text + "  ";
            }

            return newValue;
        }
    }
}
