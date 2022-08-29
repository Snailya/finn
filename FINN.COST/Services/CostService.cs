using FINN.CORE.Interfaces;
using FINN.COST.Models;
using FINN.SHAREDKERNEL.UseCases;
using Microsoft.Extensions.Logging;

namespace FINN.COST.Services;

public class CostService
{
    private readonly ILogger<CostService> _logger;
    private readonly IRepository<Cost> _repository;

    public CostService(ILogger<CostService> logger, IRepository<Cost> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public decimal EstimateCost(IEnumerable<GeometryDto> dto)
    {
        throw new NotImplementedException();
    }
}