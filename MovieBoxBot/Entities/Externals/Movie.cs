#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Entities.Externals;

public class Movie
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("url")]
    public string Url;

    [JsonProperty("imdb_code")]
    public string ImdbCode;

    [JsonProperty("title")]
    public string Title;

    [JsonProperty("title_english")]
    public string TitleEnglish;

    [JsonProperty("title_long")]
    public string TitleLong;

    [JsonProperty("slug")]
    public string Slug;

    [JsonProperty("year")]
    public int Year;

    [JsonProperty("rating")]
    public double Rating;

    [JsonProperty("runtime")]
    public int Runtime;

    [JsonProperty("genres")]
    public List<string> Genres;

    [JsonProperty("summary")]
    public string Summary;

    [JsonProperty("description_full")]
    public string DescriptionFull;

    [JsonProperty("synopsis")]
    public string Synopsis;

    [JsonProperty("yt_trailer_code")]
    public string YtTrailerCode;

    [JsonProperty("language")]
    public string Language;

    [JsonProperty("mpa_rating")]
    public string MpaRating;

    [JsonProperty("background_image")]
    public string BackgroundImage;

    [JsonProperty("background_image_original")]
    public string BackgroundImageOriginal;

    [JsonProperty("small_cover_image")]
    public string SmallCoverImage;

    [JsonProperty("medium_cover_image")]
    public string MediumCoverImage;

    [JsonProperty("large_cover_image")]
    public string LargeCoverImage;

    [JsonProperty("state")]
    public string State;

    [JsonProperty("torrents")]
    public List<Torrent> Torrents;

    [JsonProperty("date_uploaded")]
    public string DateUploaded;

    [JsonProperty("date_uploaded_unix")]
    public int DateUploadedUnix;
}
