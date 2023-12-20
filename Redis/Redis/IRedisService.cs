using Data;

namespace Redis.Redis;

public interface IRedisService
{
    Task<string?> GetStatusCode(Link link);
}