using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Redis;

namespace Redis;

public static class Module
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRedisService, RedisService>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "redis";
            options.InstanceName = "redis";
        });
        return services;
    }
}