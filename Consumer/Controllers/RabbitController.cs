using System.Text;
using Microsoft.AspNetCore.Mvc;
using Rabbit.RabbitMQ;
using RabbitMQ.Client;

namespace Consumer.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RabbitController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqService;

    public RabbitController(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }
    
    [HttpPost]
    public IActionResult SendMessage()
    {
        using var connection = _rabbitMqService.CreateChannel();
        using var model = connection.CreateModel();
        var body = Encoding.UTF8.GetBytes("Hi");
        model.BasicPublish("UserExchange",
            string.Empty,
            basicProperties: null,
            body: body);

        return Ok();
    }
}