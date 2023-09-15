using Pastel;
using System.Drawing;

namespace MovieBoxBot.Utils.Client;

public class HttpClientService<T> where T : class
{
    private readonly HttpClient _httpClient;

    public HttpClientService() => _httpClient = new HttpClient();

    public async Task<T?> GetAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            return await response.ReadContentAs<T>();
        }
        catch(Exception e)
        {
            Console.WriteLine("----\nException: {0}".Pastel(Color.Red), e.Message);
            return default;
        }        
    }
}
