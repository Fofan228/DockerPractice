namespace Consumer.RabbitMq.Consumer;

public interface IConsumerService
{
    Task ReadMessages();
}