using System.Text.Json;
using FINN.API.Dtos;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BlocksController : ControllerBase
{
    private readonly IBroker _broker;
    private readonly ILogger<BlocksController> _logger;

    public BlocksController(ILogger<BlocksController> logger, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PaginationFilter filter)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(List), filter);

        if (filter.PageNumber == 0 && filter.PageSize != 0)
            return BadRequest("Page number can't be 0 if page size is specified.");

        if (filter.PageNumber != 0 && filter.PageSize == 0)
            return BadRequest("Page size can't be 0 if page number is specified ");

        var response =
            JsonSerializer.Deserialize<PagedResponse<IEnumerable<BlockDefinitionDto>>>(await _broker.SendAsync(
                RoutingKeys.DxfService.ListBlockDefinitions,
                filter.ToJson()));
        if (response != null)
            return Ok(filter.PageNumber == 0 || filter.PageSize == 0
                ? new Response<IEnumerable<BlockDefinitionDto>>(response.Message, response.Code, response.Data)
                : response);

        return BadRequest();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Download), id);

        var response = JsonSerializer.Deserialize<Response<string>>(
            await _broker.SendAsync(RoutingKeys.DxfService.DownloadBlockFile, id.ToString()));

        if (response is { Code: 0, Data: { } })
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(response.Data!);
            var file = File(bytes, "text/plain", Path.GetFileName(response.Data));

            _logger.LogInformation("[{DateTime}] Download file prepared. File Name: {FileName}. Size: {Size} ",
                DateTime.Now,
                file.FileDownloadName, file.FileContents.Length);
            return file;
        }

        return BadRequest();
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add([FromForm] UploadBlockFileDto dto)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Add), dto);

        // do not allow empty file
        if (dto.File.Length <= 0) return BadRequest();

        // persist file to temp storage, notice that the temp storage is regard as unsafe
        // todo: validation
        var filePath = Path.GetTempFileName();
        await using var stream = System.IO.File.Create(filePath);
        await dto.File.CopyToAsync(stream);
        stream.Close();

        var request = new AddBlockDefinitionsDto
        {
            BlockNames = dto.BlockNames,
            Filename = filePath
        };
        var response = JsonSerializer.Deserialize<Response<IEnumerable<BlockDefinitionDto>>>(
            await _broker.SendAsync(RoutingKeys.DxfService.AddBlockDefinitions, request.ToJson()));
        return Ok(response);
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Delete), id);

        var response = JsonSerializer.Deserialize<Response>(
            await _broker.SendAsync(RoutingKeys.DxfService.DeleteBlockDefinition, id.ToString()));
        return Ok(response);
    }
}