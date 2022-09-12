using Newtonsoft.Json;

namespace MovieBoxBot.Utils.Client;

public static class HttpClientExtension
{
    public static async Task<T?> ReadContentAs<T>(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonConvert.DeserializeObject<T>(dataAsString);
    }
}