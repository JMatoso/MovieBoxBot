#nullable disable

namespace MovieBoxBot.Utils.Client;

public class HttpClientService<T> where T : class
{
    private readonly HttpClient _httpClient;

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<IEnumerable<T>> GetListAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);

        return await response.ReadContentAs<IEnumerable<T>>(); ;
    }

    public async Task<T> GetAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        return await response.ReadContentAs<T>();
    }
}
