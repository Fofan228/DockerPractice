using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Data;
using Rabbit.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.RabbitMq.Consumer;

public class ConsumerService : IConsumerService, IDisposable
{
    private readonly IModel _model;
    private readonly IConnection _connection;
    private const string QueueName = "Links";
    
    public ConsumerService(IRabbitMqService rabbitMqService)
    {
        _connection = rabbitMqService.CreateChannel();
        _model = _connection.CreateModel();
        _model.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _model.ExchangeDeclare("LinkExchange", ExchangeType.Fanout, durable: true, autoDelete: false);
        _model.QueueBind(QueueName, "LinkExchange", string.Empty);
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
        var statusCode = await GetStatusCode(link.Url);
        
        using var client = new HttpClient();
        
        client.BaseAddress = new Uri("http://localhost:5000/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        link.Status = statusCode.ToString();
        
        var serializeObj = JsonSerializer.Serialize(link);
        var stringContent = new StringContent(serializeObj, Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/Links/update/", stringContent);
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception();
        }
    }

    private async Task<HttpStatusCode> GetStatusCode(string link)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(link);
        
        return response.StatusCode;
    }
}