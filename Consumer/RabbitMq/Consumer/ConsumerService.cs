using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Data;
using Rabbit.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Redis.Redis;

namespace Consumer.RabbitMq.Consumer;

public class ConsumerService : IConsumerService, IDisposable
{
    private const string QueueName = "Links";
    private const string Exchange = "LinkExchange";

    private readonly IModel _model;
    private readonly IConnection _connection;
    private readonly IRedisService _redisService;
    private readonly IRabbitMqService _rabbitMqService;

    public ConsumerService(IRabbitMqService rabbitMqService, IRedisService redisService)
    {
        _rabbitMqService = rabbitMqService;
        _redisService = redisService;
        _connection = _rabbitMqService.CreateChannel();
        _model = _connection.CreateModel();
        _model.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _model.ExchangeDeclare(Exchange, ExchangeType.Fanout, durable: true, autoDelete: false);
        _model.QueueBind(QueueName, Exchange, string.Empty);
    }

    public async Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var link = Encoding.UTF8.GetString(body);
            var jsonLink = JsonSerializer.Deserialize<Link>(link);
            await RunAsyncGet(jsonLink);
            await Task.CompletedTask;
            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume(QueueName, false, consumer);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_model.IsOpen)
            _model.Close();
        if (_connection.IsOpen)
            _connection.Close();
    }

    private async Task RunAsyncGet(Link link)
    {
        using var client = new HttpClient();

        client.BaseAddress = new Uri("http://localhost:5000/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var linkInCache = await _redisService.GetStatusCode(link.Id);

        if (linkInCache == null)
            throw new Exception("Entity with id does not exist in Database");

        link.Status = linkInCache;

        var serializeObj = JsonSerializer.Serialize(link);
        var stringContent = new StringContent(serializeObj, Encoding.UTF8, "application/json");

        var response = await client.PutAsync("/Links/update/", stringContent);
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception();
        }
    }
}