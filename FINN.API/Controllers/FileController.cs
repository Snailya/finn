using System.Text;
using System.Text.Json;
using FINN.API.Contexts;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly IBroker _broker;
    private readonly IDbContextFactory<JobContext> _factory;
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger, IBroker broker, IDbContextFactory<JobContext> factory)
    {
        _logger = logger;
        _broker = broker;
        _factory = factory;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length > 0)
        {
            var filePath = Path.GetTempFileName();
            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            await using var context = await _factory.CreateDbContextAsync();
            var job = new Job(filePath);
            context.Add(job);
            await context.SaveChangesAsync();

            _broker.Send(RoutingKey.Read,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ReaderDto(job.Id, filePath))));
            return AcceptedAtAction(nameof(CheckStatus), new { jobId = job.Id }, job);
        }

        // Process uploaded files
        // Don't rely on or trust the FileName property without validation.

        return BadRequest();
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckStatus([FromQuery] int jobId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var job = await context.Jobs.FindAsync(jobId);

        return job.Status switch
        {
            JobStatus.Error => Accepted(job),
            JobStatus.Ready => AcceptedAtAction(nameof(Download), new { jobId = job.Id }, job),
            _ => AcceptedAtAction(nameof(CheckStatus), new { jobId = job.Id }, job)
        };
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] int jobId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var job = await context.Jobs.FindAsync(jobId);
        var bytes = await System.IO.File.ReadAllBytesAsync(job.Output);
        return File(bytes, "text/plain", Path.GetFileName(job.Output));
    }
}