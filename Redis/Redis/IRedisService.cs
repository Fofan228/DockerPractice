namespace Redis.Redis;

public interface IRedisService
{
    Task<string?> GetStatusCode(long id);
}