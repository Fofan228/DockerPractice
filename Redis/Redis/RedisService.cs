using Data;
using Microsoft.Extensions.Caching.Distributed;
using Rabbit.HttpService;

namespace Redis.Redis;

public class RedisService : IRedisService
{
    private readonly IHttpService _httpService;
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache, IHttpService httpService)
    {
        _cache = cache;
        _httpService = httpService;
    }

    public async Task<string?> Get(string url, string statusCode)
    {
        var result = await _cache.GetStringAsync(url);

        if (result != null)
            return result;

        await _cache.SetStringAsync(url, statusCode, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        });
        
        return result;
    }
}