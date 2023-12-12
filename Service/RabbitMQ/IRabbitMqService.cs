using RabbitMQ.Client;

namespace Rabbit.RabbitMQ;

public interface IRabbitMqService
{
    void SendMessage(object obj);
    void SendMessage(string message);
    IConnection CreateChannel();
}