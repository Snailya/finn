using System.Text.Json;
using FINN.API.Dtos;
using FINN.CORE.Interfaces;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Management;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BlockController : ControllerBase
{
    private readonly IBroker _broker;
    private readonly ILogger<BlockController> _logger;

    public BlockController(ILogger<BlockController> logger, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadBlockFileDto dto)
    {
        if (dto.File.Length <= 0) return BadRequest();

        var filePath = Path.GetTempFileName();
        await using var stream = System.IO.File.Create(filePath);
        await dto.File.CopyToAsync(stream);
        stream.Close();

        var message = new InsertBlockRequestDto { Filename = filePath, Names = dto.BlockNames }.ToJson();
        var response =
            JsonSerializer.Deserialize<Response<InsertBlockResponseDto>>(
                await _broker.SendAsync(RoutingKeys.InsertBlock, message));
        return Accepted(response);
    }
}