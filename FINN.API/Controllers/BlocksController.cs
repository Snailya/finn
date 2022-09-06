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

    public BlocksController(IBroker broker)
    {
        _broker = broker;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PaginationFilter filter)
    {
        if ((filter.PageNumber == 0 && filter.PageSize != 0))
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
        var response = JsonSerializer.Deserialize<Response<BlockDefinitionDto>>(
            await _broker.SendAsync(RoutingKeys.DxfService.GetBlockDefinition, id.ToString()));

        if (response?.Code == 0)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(response.Data.Filename!);
            return File(bytes, "text/plain", response.Data.Name);
        }

        return BadRequest();
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add([FromForm] UploadBlockFileDto dto)
    {
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
        var response = JsonSerializer.Deserialize<Response>(
            await _broker.SendAsync(RoutingKeys.DxfService.DeleteBlockDefinition, id.ToString()));
        return Ok(response);
    }
}