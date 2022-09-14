#nullable disable

using Newtonsoft.Json;

namespace MovieBoxBot.Entities.Externals;

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
