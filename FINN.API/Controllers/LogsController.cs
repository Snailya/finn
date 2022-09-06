using FINN.API.Dtos;
using FINN.API.Models;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;
    private readonly IRepository<RequestLog> _repository;

    public LogsController(ILogger<LogsController> logger, IRepository<RequestLog> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received", DateTime.Now,
            nameof(List));

        var logs = await _repository.ListAsync();
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