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

    public CostDto EstimateCost(IEnumerable<GeometryDto> dto)
    {
        return new CostDto
        {
            Platform = (from platform in dto.Where(x => x.Type == "platform")
                let unitPrice = (platform.ZPosition > 5000 ? 130 : 120) * 1.2E-9M
                let area = platform.XLength * platform.YLength
                select unitPrice * (decimal)area).Sum()
        };
    }
}