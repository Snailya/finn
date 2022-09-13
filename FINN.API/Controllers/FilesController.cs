using System.Text.Json;
using FINN.API.Models;
using FINN.CORE;
using FINN.CORE.Extensions;
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
    private readonly IRepositoryFactory<RequestLog> _factory;
    private readonly ILogger<FilesController> _logger;

    public FilesController(ILogger<FilesController> logger, IBroker broker, IRepositoryFactory<RequestLog> factory)
    {
        _logger = logger;
        _broker = broker;
        _factory = factory;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Upload), file);

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
            ".xlsx" => await HandleXlsxUpload(input, file.FileName),
            ".dxf" => await HandleDxfUpload(input, file.FileName)
        };
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Download), id);

        using var repository = _factory.CreateReadRepository();
        var log = await repository.GetByIdAsync(id);
        if (log is { Status: "done", RequestType: "layout" })
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(log.Output!);
            var file = File(bytes, "text/plain", Path.GetFileName(log.Output));

            _logger.LogInformation("[{DateTime}] Download file prepared. File Name: {FileName}. Size: {Size} ",
                DateTime.Now,
                file.FileDownloadName, file.FileContents.Length);

            return file;
        }

        return BadRequest();
    }

    private async Task<IActionResult> HandleDxfUpload(string input, string origin)
    {
        _logger.LogInformation("Route to handle as dxf. The tmp file is stored at path: {Path}", input);

        using var repository = _factory.CreateRepository();
        // create log
        var log = new RequestLog
        {
            Origin = origin,
            Input = input,
            RequestType = "cost",
            Status = "pending"
        };
        await repository.AddAsync(log);
        await repository.SaveChangesAsync();

        // ask microservice to handle
        var geo = JsonSerializer.Deserialize<Response<IEnumerable<GeometryDto>>>(
            await _broker.SendAsync(RoutingKeys.DxfService.ReadLayout, input))!;
        if (geo.Code != 0)
        {
            log.Output = geo.Message;
            log.Status = "error";
            await repository.UpdateAsync(log);
            await repository.SaveChangesAsync();

            return Ok(geo);
        }

        var cost =
            JsonSerializer.Deserialize<Response<IEnumerable<CostDto>>>(await _broker.SendAsync(
                RoutingKeys.CostService.EstimateCost,
                geo.Data.ToJson()))!;
        if (cost.Code != 0)
        {
            log.Output = cost.Message;
            log.Status = "error";
            await repository.UpdateAsync(log);
            await repository.SaveChangesAsync();
            return Ok(cost);
        }

        log.Output = cost.Data.ToJson();
        log.Status = "done";
        await repository.UpdateAsync(log);
        await repository.SaveChangesAsync();
        return Ok(cost);
    }

    private async Task<IActionResult> HandleXlsxUpload(string input, string origin)
    {
        _logger.LogInformation("Route to handle as xlsx. The tmp file is stored at path: {Path}", input);

        using var repository = _factory.CreateRepository();

        // create log
        var log = new RequestLog
        {
            Origin = origin,
            Input = input,
            RequestType = "layout",
            Status = "pending"
        };
        await repository.AddAsync(log);
        await repository.SaveChangesAsync();

        // ask microservice to handle
        var getLayoutResponse = JsonSerializer.Deserialize<Response<LayoutDto>>(
            await _broker.SendAsync(RoutingKeys.ExcelService.GetLayout, input));
        if (getLayoutResponse.Code != 0)
        {
            // update status
            log.Output = getLayoutResponse.Message;
            log.Status = "error";
            await repository.UpdateAsync(log);
            await repository.SaveChangesAsync();

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
            await repository.UpdateAsync(log);
            await repository.SaveChangesAsync();

            return Ok(drawLayoutResponse);
        }

        // convert to download link
        log.Output = drawLayoutResponse.Data;
        log.Status = "done";
        await repository.UpdateAsync(log);
        await repository.SaveChangesAsync();

        return AcceptedAtAction(nameof(Download), new { id = log.Id }, new Response<int>("", 0, log.Id));
    }
}