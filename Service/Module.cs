using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RabbitMQ;

namespace Rabbit;

public static class Module
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "redis";
            options.InstanceName = "local";
        });
        return services;
    }
}