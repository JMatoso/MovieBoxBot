#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Entities.Externals;

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
