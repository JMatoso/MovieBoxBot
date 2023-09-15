using MovieBoxBot.Data;
using MovieBoxBot.Entities.Externals;
using MovieBoxBot.Models;
using MovieBoxBot.Utils.Client;
using Pastel;
using System.Drawing;

namespace MovieBoxBot.Utils;

internal class Actions
{
    private static readonly HttpClientService<Root> _httpClientService = new();
    private static readonly string _baseUrl = "https://yts.mx/api/v2/list_movies.json?limit=4&page={0}&query_term={1}";

    public static TextMessage Help() => new()
    {
        Text = "I am the <a href =\"http://www.moviebox.site\">MovieBox</a> website bot, I will help you to search, see the details and download torrents of the movies you want." +
            Environment.NewLine + Environment.NewLine + "I was created by José Matoso, in order to help movie lovers getting torrents easier.",
        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
    };

    public static TextMessage Start(string name) => new()
    {
        Text = $"Hey, {name}! Tell me what you want to do typing one of the commands I know."
    };

    public static PhotoMessageModel? List(int page, string queryTerm = "")
    {
        try
        {
            var result = _httpClientService.GetAsync(string.Format(_baseUrl, page, queryTerm.Trim())).Result;     
            if(result is null) return default;

            UserActivity.ActualPage = page;
            UserActivity.SearchKeyword = queryTerm.Trim();

            var photoMessageModel = new PhotoMessageModel()
            {
                MoviesCount = result.Data.MovieCount,
                Message = $"I've got {result.Data.MovieCount} movie(s) for you.\nPage: {page}",
                Pages = (int)Math.Ceiling(Convert.ToDecimal(result.Data.MovieCount / result.Data.Limit))
            };

            result.Data.Movies?.ForEach((movie) =>
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

            return photoMessageModel;
        }
        catch(Exception e)
        {
            Console.WriteLine("----\nServer Error: {0}".Pastel(Color.Red), e.Message);
        }

        return default;
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
