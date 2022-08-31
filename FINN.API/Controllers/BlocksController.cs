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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = JsonSerializer.Deserialize<Response<BlockDefinitionDto>>(
            await _broker.SendAsync(RoutingKeys.DxfService.GetBlockDefinition, id.ToString()));
        return Ok(response);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add([FromForm] BlockFileDto dto)
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

        // // send to dispatcher
        // var request = new DispatchRequest
        // {
        //     Task = DispatchTask.UploadBlocks,
        //     Data = new UploadOrUpdateBlocksDto { Filename = filePath, Names = dto.BlockNames }.ToJson()
        // };
        // var response =
        //     JsonSerializer.Deserialize<Response<DispatchResult>>(
        //         await _broker.SendAsync(RoutingKeys.Dispatch, request.ToJson()));
        // if (response is { Code: not 0 }) return BadRequest();
        //
        // return Accepted(response);
    }

    // [HttpPut("update")]
    // public async Task<IActionResult> Update([FromForm] BlockFileDto dto)
    // {
    // // do not allow empty file
    // if (dto.File.Length <= 0) return BadRequest();
    //
    // // persist file to temp storage, notice that the temp storage is regard as unsafe
    // // todo: validation
    // var filePath = Path.GetTempFileName();
    // await using var stream = System.IO.File.Create(filePath);
    // await dto.File.CopyToAsync(stream);
    // stream.Close();

    // // send to dispatcher
    // var request = new DispatchRequest
    // {
    //     Task = DispatchTask.UpdateBlocks,
    //     Data = new UploadOrUpdateBlocksDto { Filename = filePath, Names = dto.BlockNames }.ToJson()
    // };
    // var response =
    //     JsonSerializer.Deserialize<Response<DispatchResult>>(
    //         await _broker.SendAsync(RoutingKeys.Dispatch, request.ToJson()));
    // if (response is { Code: not 0 }) return BadRequest();
    //
    // return Accepted(response);
    // }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = JsonSerializer.Deserialize<Response>(
            await _broker.SendAsync(RoutingKeys.DxfService.DeleteBlockDefinition, id.ToString()));
        return Ok(response);

        // // send to dispatcher
        // var request = new DispatchRequest
        // {
        //     Task = DispatchTask.DeleteBlock,
        //     Data = id.ToString()
        // };
        // var response =
        //     JsonSerializer.Deserialize<Response<DispatchResult>>(
        //         await _broker.SendAsync(RoutingKeys.Dispatch, request.ToJson()));
        // if (response is { Code: not 0 }) return BadRequest();
        //
        // return Accepted(response);
    }
}