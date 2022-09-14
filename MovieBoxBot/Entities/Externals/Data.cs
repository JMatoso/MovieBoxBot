#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Entities.Externals;

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
