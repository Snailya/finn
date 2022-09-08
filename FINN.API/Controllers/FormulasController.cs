using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FINN.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FormulasController : ControllerBase
{
    private readonly ILogger<FormulasController> _logger;
    private readonly IBroker _broker;

    public FormulasController(ILogger<FormulasController> logger, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received", DateTime.Now,
            nameof(List));


        var response =
            JsonSerializer.Deserialize<Response<IEnumerable<FormulaDto>>>(
                await _broker.SendAsync(RoutingKeys.CostService.ListFormulas, ""));
        if (response != null)
            return Ok(response);

        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromForm] FormulaDto dto)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Add), dto);

        var response =
            JsonSerializer.Deserialize<Response<FormulaDto>>(
                await _broker.SendAsync(RoutingKeys.CostService.AddFormula, dto.ToJson()));
        if (response != null)
            return Ok(response);

        return BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] FormulaDto dto)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Update), dto);

        var response =
            JsonSerializer.Deserialize<Response<FormulaDto>>(
                await _broker.SendAsync(RoutingKeys.CostService.UpdateFormula, dto.ToJson()));
        if (response != null)
            return Ok(response);

        return BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("[{DateTime}] Request {Action} received. Parameter: {Parameter}", DateTime.Now,
            nameof(Delete), id);

        var response = JsonSerializer.Deserialize<Response>(
            await _broker.SendAsync(RoutingKeys.CostService.DeleteFormula, id.ToString()));
        return Ok(response);
    }
}