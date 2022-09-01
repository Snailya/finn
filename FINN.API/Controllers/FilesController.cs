using System.Text.Json;
using FINN.API.Models;
using FINN.CORE;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly IBroker _broker;
    private readonly ILogger<FilesController> _logger;
    private readonly IRepository<RequestLog> _repository;

    public FilesController(ILogger<FilesController> logger, IBroker broker, IRepository<RequestLog> repository)
    {
        _logger = logger;
        _broker = broker;
        _repository = repository;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // not allow empty body
        var extension = Path.GetExtension(file.FileName);
        if (file.Length <= 0 || (extension != ".dxf" && extension != ".xlsx")) return BadRequest();

        // persist to local storage
        var input = Path.GetTempFileName();
        await using var stream = System.IO.File.Create(input);
        await file.CopyToAsync(stream);
        stream.Close();

        return extension switch
        {
            ".xlsx" => await HandleXlsxUpload(input),
            ".dxf" => await HandleDxfUpload(input)
        };
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        var log = await _repository.GetByIdAsync(id);
        if (log is { Status: "done" })
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(log.Output!);
            return File(bytes, "text/plain", Path.GetFileName(log.Output));
        }

        return BadRequest();
    }

    private async Task<IActionResult> HandleDxfUpload(string input)
    {
        // create log
        var log = new RequestLog
        {
            Input = input,
            Ip = Request.Host.Host,
            RequestType = "cost",
            Status = "pending"
        };
        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();

        // ask microservice to handle
        var geo = JsonSerializer.Deserialize<Response<IEnumerable<GeometryDto>>>(
            await _broker.SendAsync(RoutingKeys.DxfService.ReadLayout, input))!;
        if (geo.Code != 0)
        {
            log.Output = geo.Message;
            log.Status = "error";
            await _repository.UpdateAsync(log);
            await _repository.SaveChangesAsync();

            return Ok(geo);
        }

        var cost =
            JsonSerializer.Deserialize<Response<CostDto>>(await _broker.SendAsync(
                RoutingKeys.CostService.EstimateCost,
                geo.Data.ToJson()))!;
        if (cost.Code != 0)
        {
            log.Output = cost.Message;
            log.Status = "error";
            await _repository.UpdateAsync(log);
            await _repository.SaveChangesAsync();
            return Ok(cost);
        }

        log.Output = cost.Data.ToJson();
        log.Status = "done";
        await _repository.UpdateAsync(log);
        await _repository.SaveChangesAsync();
        return Ok(cost);
    }

    private async Task<IActionResult> HandleXlsxUpload(string input)
    {
        // create log
        var log = new RequestLog
        {
            Input = input,
            Ip = Request.Host.Host,
            RequestType = "layout",
            Status = "pending"
        };
        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();

        // ask microservice to handle
        var getLayoutResponse = JsonSerializer.Deserialize<Response<LayoutDto>>(
            await _broker.SendAsync(RoutingKeys.ExcelService.GetLayout, input));
        if (getLayoutResponse.Code != 0)
        {
            // update status
            log.Output = getLayoutResponse.Message;
            log.Status = "error";
            await _repository.UpdateAsync(log);
            await _repository.SaveChangesAsync();

            return Ok(getLayoutResponse);
        }

        var drawLayoutResponse =
            JsonSerializer.Deserialize<Response<string>>(await _broker.SendAsync(RoutingKeys.DxfService.DrawLayout,
                getLayoutResponse.Data.ToJson()));
        if (drawLayoutResponse.Code != 0)
        {
            // update status
            log.Output = drawLayoutResponse.Message;
            log.Status = "error";
            await _repository.UpdateAsync(log);
            await _repository.SaveChangesAsync();

            return Ok(drawLayoutResponse);
        }

        // convert to download link
        log.Output = drawLayoutResponse.Data;
        log.Status = "done";
        await _repository.UpdateAsync(log);
        await _repository.SaveChangesAsync();

        return AcceptedAtAction(nameof(Download), new { id = log.Id }, new Response<int>("", 0, log.Id));
    }
}