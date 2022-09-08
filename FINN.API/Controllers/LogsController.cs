using FINN.API.Dtos;
using FINN.API.Models;
using FINN.CORE.Models;
using FINN.PLUGINS.EFCORE;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;
    private readonly IRepositoryFactory<RequestLog> _factory;

    public LogsController(ILogger<LogsController> logger, IRepositoryFactory<RequestLog> factory)
    {
        _logger = logger;
        _factory = factory;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received", DateTime.Now,
            nameof(List));

        using var repository = _factory.CreateRepository();
        var logs = await repository.ListAsync();
        var response = new Response<IEnumerable<RequestLogDto>>("", 0, logs.Select(x => new RequestLogDto
        {
            Id = x.Id,
            Created = x.Created?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"),
            Type = x.RequestType,
            Status = x.Status,
            Input = x.Input,
            Output = x.Output
        }));
        return Ok(response);
    }
}