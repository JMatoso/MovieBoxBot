#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Entities.Externals;

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
