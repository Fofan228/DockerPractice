namespace Rabbit.HttpService;

public class HttpService : IHttpService
{
    public async Task<string> GetStatusCodeAsync(string url)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);

        return response.StatusCode.ToString();
    }
}