using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Rabbit.RabbitMQ;

public class RabbitMqService : IRabbitMqService
{
    public IConnection CreateChannel()
    {
        var connection = new ConnectionFactory
        {
            HostName = "localhost",
            DispatchConsumersAsync = true,
            RequestedHeartbeat = new TimeSpan(60),
            Ssl =
            {
                ServerName = "localhost",
                Enabled = true
            }
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