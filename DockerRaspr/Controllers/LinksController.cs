using Data;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rabbit.RabbitMQ;

namespace DockerRaspr.Controllers;

[ApiController]
[Route("[controller]")]
public class LinksController : ControllerBase
{
    private readonly IRepositoryLink _repositoryLink;
    private readonly IRabbitMqService _rabbitMqService;
    
    public LinksController(IRabbitMqService rabbitMqService, IRepositoryLink repositoryLink)
    {
        _rabbitMqService = rabbitMqService;
        _repositoryLink = repositoryLink;
    }

    [HttpGet("get/{id:long}")]
    public async Task<ActionResult<Link>> Get(long id)
    {
        var entityFromDb = await _repositoryLink.GetLinkByIdAsync(id);
        
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

        await _repositoryLink.InsertAsync(entity);

        await _repositoryLink.SaveAsync();

        var linkToQueue = await _repositoryLink.GetFirstInInvertLinksAsync();

        if (linkToQueue != null) 
            _rabbitMqService.SendMessage(linkToQueue);

        return link.Id;
    }

    [HttpPut("update/")]
    public async Task<ActionResult> Update([FromBody] Link link)
    {
        var entityFromDb = await _repositoryLink.GetLinkByIdAsync(link.Id);

        if (entityFromDb is not null)
        {
            entityFromDb.Id = link.Id;
            entityFromDb.Url = link.Url;
            entityFromDb.Status = link.Status;

            await _repositoryLink.SaveAsync();
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
        var entityFromDb = await _repositoryLink.GetLinkByIdAsync(id);

        if (entityFromDb is not null)
            _repositoryLink.Delete(entityFromDb);
        else
            return BadRequest("Database not have entity with this id");

        await _repositoryLink.SaveAsync();

        return Ok();
    }
}