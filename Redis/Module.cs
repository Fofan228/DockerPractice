using Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Redis;

namespace Redis;

public static class Module
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IRedisService, RedisService>();
        return services;
    }
}