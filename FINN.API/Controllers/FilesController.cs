using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly IBroker _broker;

    private readonly ILogger<FilesController> _logger;
    // private readonly IRepository<LayoutJob> _repository;

    public FilesController(ILogger<FilesController> logger, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
        // _repository = repository;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadXlsx(IFormFile file)
    {
        if (file.Length > 0)
        {
            var filePath = Path.GetTempFileName();
            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            stream.Close();

            var getLayoutResponse = JsonSerializer.Deserialize<Response<LayoutDto>>(
                await _broker.SendAsync(RoutingKeys.ExcelService.GetLayout, filePath));
            if (getLayoutResponse.Code != 0) return Ok(getLayoutResponse);

            var drawLayoutResponse =
                JsonSerializer.Deserialize<Response<string>>(await _broker.SendAsync(RoutingKeys.DxfService.DrawLayout,
                    getLayoutResponse.Data.ToJson()));
            if (drawLayoutResponse.Code != 0) return Ok(drawLayoutResponse);

            // convert to download link
            var output = drawLayoutResponse.Data;
            var bytes = await System.IO.File.ReadAllBytesAsync(output);
            return File(bytes, "text/plain", Path.GetFileName(output));
        }

        return BadRequest();

        // if (file.Length > 0)
        // {
        //     var filePath = Path.GetTempFileName();
        //     await using var stream = System.IO.File.Create(filePath);
        //     await file.CopyToAsync(stream);
        //
        //     var job = await _repository.AddAsync(new LayoutJob(filePath));
        //     await _repository.SaveChangesAsync();
        //
        //     _broker.Send(RoutingKeys.ReadXlsx,
        //         JsonSerializer.Serialize(new ReadRequestDto(job.Id, filePath)));
        //     return AcceptedAtAction(nameof(CheckStatus), new { jobId = job.Id }, job);
        // }
        //
        // // Process uploaded files
        // // Don't rely on or trust the FileName property without validation.
        //
        // return BadRequest();
    }

    // [HttpGet("check")]
    // public async Task<IActionResult> CheckStatus([FromQuery] int jobId)
    // {
    //     var job = await _repository.GetByIdAsync(jobId);
    //
    //     return job.Status switch
    //     {
    //         JobStatus.Error => Accepted(job),
    //         JobStatus.Ready => AcceptedAtAction(nameof(Download), new { jobId = job.Id }, job),
    //         _ => AcceptedAtAction(nameof(CheckStatus), new { jobId = job.Id }, job)
    //     };
    // }
    //
    // [HttpGet("download")]
    // public async Task<IActionResult> Download([FromQuery] int jobId)
    // {
    //     var job = await _repository.GetByIdAsync(jobId);
    //
    //     var bytes = await System.IO.File.ReadAllBytesAsync(job.Output);
    //     return File(bytes, "text/plain", Path.GetFileName(job.Output));
    // }
}