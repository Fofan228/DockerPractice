using System.Net;
using Data;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Redis.Redis;

public class RedisService : IRedisService
{
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetStatusCode(Link link)
    {
        var linkString = await _cache.GetStringAsync(link.Id.ToString());

        if (linkString != null)
            return linkString;

        var statusCode = await GetStatusCodeAsync(link.Url);

        linkString = statusCode.ToString();
        
        await _cache.SetStringAsync(link.Id.ToString(), linkString, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        });
        
        return linkString;
    }
    
    private async Task<HttpStatusCode> GetStatusCodeAsync(string url)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);

        return response.StatusCode;
    }
}