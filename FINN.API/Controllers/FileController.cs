using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos.Reader;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly IBroker _broker;
    private readonly ILogger<FileController> _logger;
    private readonly IRepository<Job> _repository;

    public FileController(ILogger<FileController> logger, IBroker broker, IRepository<Job> repository)
    {
        _logger = logger;
        _broker = broker;
        _repository = repository;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length > 0)
        {
            var filePath = Path.GetTempFileName();
            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var job = await _repository.AddAsync(new Job(filePath));
            await _repository.SaveChangesAsync();

            _broker.Send(RoutingKeys.ReadXlsx,
                JsonSerializer.Serialize(new ReadRequestDto(job.Id, filePath)));
            return AcceptedAtAction(nameof(CheckStatus), new { jobId = job.Id }, job);
        }

        // Process uploaded files
        // Don't rely on or trust the FileName property without validation.

        return BadRequest();
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckStatus([FromQuery] int jobId)
    {
        var job = await _repository.GetByIdAsync(jobId);

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
        var job = await _repository.GetByIdAsync(jobId);

        var bytes = await System.IO.File.ReadAllBytesAsync(job.Output);
        return File(bytes, "text/plain", Path.GetFileName(job.Output));
    }
}