using Data;

namespace Redis.Redis;

public interface IRedisService
{
    Task<string?> Get(string url, string statusCode);
}