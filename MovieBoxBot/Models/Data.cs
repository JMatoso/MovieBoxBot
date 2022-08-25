#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        [JsonProperty("movie_count")]
        public int MovieCount;

        [JsonProperty("limit")]
        public int Limit;

        [JsonProperty("page_number")]
        public int PageNumber;

        [JsonProperty("movies")]
        public List<Movie> Movies;
    }

    public class Meta
    {
        [JsonProperty("server_time")]
        public int ServerTime;

        [JsonProperty("server_timezone")]
        public string ServerTimezone;

        [JsonProperty("api_version")]
        public int ApiVersion;

        [JsonProperty("execution_time")]
        public string ExecutionTime;
    }

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

    public class Root
    {
        [JsonProperty("status")]
        public string Status;

        [JsonProperty("status_message")]
        public string StatusMessage;

        [JsonProperty("data")]
        public Data Data;

        [JsonProperty("@meta")]
        public Meta Meta;
    }

    public class Torrent
    {
        [JsonProperty("url")]
        public string Url;

        [JsonProperty("hash")]
        public string Hash;

        [JsonProperty("quality")]
        public string Quality;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("seeds")]
        public int Seeds;

        [JsonProperty("peers")]
        public int Peers;

        [JsonProperty("size")]
        public string Size;

        [JsonProperty("size_bytes")]
        public long SizeBytes;

        [JsonProperty("date_uploaded")]
        public string DateUploaded;

        [JsonProperty("date_uploaded_unix")]
        public int DateUploadedUnix;
    }
}
