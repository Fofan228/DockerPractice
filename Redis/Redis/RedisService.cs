using System.Net;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Redis.Redis;

public class RedisService : IRedisService
{
    private readonly IRepositoryLink _repositoryLink;
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache, IRepositoryLink repositoryLink)
    {
        _cache = cache;
        _repositoryLink = repositoryLink;
    }

    public async Task<string?> GetStatusCode(long id)
    {
        var linkString = await _cache.GetStringAsync(id.ToString());

        if (linkString != null)
            return linkString;
        
        var link = await _repositoryLink.GetLinkByIdAsync(id);

        if (link == null) 
            return null;
        
        var statusCode = await GetStatusCodeAsync(link.Url);

        linkString = statusCode.ToString();
        
        await _cache.SetStringAsync(link.Id.ToString(), linkString, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        });
        
        return linkString;
    }
    
    private async Task<HttpStatusCode> GetStatusCodeAsync(string link)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(link);

        return response.StatusCode;
    }
}