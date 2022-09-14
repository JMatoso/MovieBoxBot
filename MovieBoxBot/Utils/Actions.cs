using MovieBoxBot.Models;
using MovieBoxBot.Utils.Client;
using MovieBoxBot.Entities.Externals;
using MovieBoxBot.Data;

namespace MovieBoxBot.Utils;

internal class Actions
{
    private static readonly HttpClientService<Root> _httpClientService = new();
    private static readonly string _baseUrl = "https://yts.mx/api/v2/list_movies.json?limit=4&page={0}&query_term={1}";

    public static TextMessage Help() => new()
    {
        Text = "I am the <a href =\"http://www.moviebox.site\">MovieBox</a> website bot, I will help you to search, see the details and download torrents of the movies you want." +
            Environment.NewLine + Environment.NewLine + "I was created by <a href =\"http://github.com/JMatoso/MovieBoxBot\">José Matoso</a>, in order to help movie lovers getting torrents easier.",
        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
    };

    public static TextMessage Start(string name) => new()
    {
        Text = $"Let's get started, {name}! Tell me what you want to do typing one of the commands I know."
    };

    public static PhotoMessageModel List(int page, string queryTerm = "")
    {
        var result = _httpClientService.GetAsync(string.Format(_baseUrl, page, queryTerm.Trim())).Result;

        var photoMessageModel = new PhotoMessageModel()
        {
            MoviesCount = result.Data.MovieCount,
            Message = $"I've got {result.Data.MovieCount} movie(s) for you.\nPage: {page}",
            Pages = (int)Math.Ceiling(Convert.ToDecimal(result.Data.MovieCount / result.Data.Limit))
        };

        UserActivity.ActualPage = page;
        UserActivity.SearchKeyword = queryTerm.Trim();

        if (result.Data.Movies is not null)
        {
            result.Data.Movies.ForEach((movie) =>
            {
                photoMessageModel.PhotoMessages.Add(new PhotoMessage()
                {
                    MovieId = movie.Id.ToString(),
                    Photo = movie.MediumCoverImage,
                    Caption = $"<b>{movie.TitleLong}</b>" +
                        Environment.NewLine + GetStringCollection(movie.Genres) +
                        Environment.NewLine + Environment.NewLine +
                        (movie.DescriptionFull.Length > 250 ? movie.DescriptionFull[..250] : movie.DescriptionFull) + "..." +
                        Environment.NewLine + Environment.NewLine + GetTorrents(movie.Torrents)
                });
            });
        }

        return photoMessageModel;
    }

    private static string GetTorrents(List<Torrent> torrents)
    {
        var list = string.Empty;

        torrents.ForEach((torrent) =>
        {
            list += $"<a href=\"{torrent.Url}\">{torrent.Type} - {torrent.Quality} - {torrent.Size}</a>" + Environment.NewLine;
        });

        return list;
    }

    private static string GetStringCollection(List<string> list)
    {
        var newValue = string.Empty;

        list.ForEach((text) => { newValue += text + "  "; });

        return newValue;
    }
}
