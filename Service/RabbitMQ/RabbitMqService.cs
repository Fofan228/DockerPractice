using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Rabbit.RabbitMQ;

public class RabbitMqService : IRabbitMqService
{
    //private bool IsRunningInContainer => bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inDocker) && inDocker;

    public IConnection CreateChannel()
    {
        var connection = new ConnectionFactory
        {
            HostName = "rabbitmq",
            DispatchConsumersAsync = true,
            RequestedHeartbeat = new TimeSpan(60),
        };
        var channel = connection.CreateConnection();
        return channel;
    }
    
    public void SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        SendMessage(message);
    }
    
    public void SendMessage(string message)
    {
        using var connection = CreateChannel();
        using var model = connection.CreateModel();
        var body = Encoding.UTF8.GetBytes(message);
        model.BasicPublish("LinkExchange",
            string.Empty,
            basicProperties: null,
            body: body);
    }
}