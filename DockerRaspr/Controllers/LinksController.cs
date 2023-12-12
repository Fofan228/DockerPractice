using Data;
using Microsoft.AspNetCore.Mvc;
using Rabbit.RabbitMQ;

namespace DockerRaspr.Controllers;

[ApiController]
[Route("[controller]")]
public class LinksController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRabbitMqService _rabbitMqService;
    
    public LinksController(ApplicationDbContext dbContext, IRabbitMqService rabbitMqService)
    {
        _dbContext = dbContext;
        _rabbitMqService = rabbitMqService;
    }

    [HttpGet("get/{id:long}")]
    public async Task<ActionResult<Link>> Get(long id)
    {
        var entityFromDb = await _dbContext.Links.FindAsync(id);
        
        if (entityFromDb is null)
            return BadRequest("Database not have entity with this id");
        
        return Ok(entityFromDb);
    }

    [HttpPost("create/")]
    public async Task<long> Create([FromBody] Link link)
    {
        var entity = new Link
        {
            Id = link.Id,
            Url = link.Url,
            Status = link.Status,
        };

        await _dbContext.Links.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        
        _rabbitMqService.SendMessage(link);

        return link.Id;
    }

    [HttpPut("update/")]
    public async Task<ActionResult> Update([FromBody] Link link)
    {
        var entityFromDb = await _dbContext.Links.FindAsync(link.Id);

        if (entityFromDb is not null)
        {
            entityFromDb.Id = link.Id;
            entityFromDb.Url = link.Url;
            entityFromDb.Status = link.Status;

            await _dbContext.SaveChangesAsync();
        }
        else
        {
            return BadRequest("Database not have entity with this id");
        }

        return Ok();
    }

    [HttpDelete("delete/{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        var entityFromDb = await _dbContext.Links.FindAsync(id);

        if (entityFromDb is not null)
            _dbContext.Links.Remove(entityFromDb);
        else
            return BadRequest("Database not have entity with this id");

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}