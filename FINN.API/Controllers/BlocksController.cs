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
        var tmp = await _broker.SendAsync(RoutingKeys.DxfService.ListBlockDefinitions, filter.ToJson());
        var response = JsonSerializer.Deserialize<PagedResponse<IEnumerable<BlockDefinitionDto>>>(
            tmp);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = JsonSerializer.Deserialize<Response<BlockDefinitionDto>>(
            await _broker.SendAsync(RoutingKeys.DxfService.GetBlockDefinition, id.ToString()));
        return Ok(response);
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