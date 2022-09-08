using FINN.COST.Models;
using FINN.PLUGINS.EFCORE;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.Extensions.Logging;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;

namespace FINN.COST.Services;

public class CostService
{
    private readonly ILogger<CostService> _logger;
    private readonly IRepositoryFactory<Formula> _factory;
    private readonly Dictionary<string, IExpression> _expressions = new();

    public CostService(ILogger<CostService> logger, IRepositoryFactory<Formula> factory)
    {
        _logger = logger;
        _factory = factory;

        Initialize();
    }

    private void Initialize()
    {
        using var repository = _factory.CreateReadRepository();
        var formulas = repository.ListAsync().GetAwaiter().GetResult();
        foreach (var formula in formulas)
        {
            var processor = new Processor().Parse(formula.Expression);
            _expressions.Add(formula.Type, processor);
        }
    }

    public IEnumerable<CostDto> EstimateCost(IEnumerable<GeometryDto> dto)
    {
        var enumerable = dto as GeometryDto[] ?? dto.ToArray();
        var costs = enumerable.GroupBy(x => x.Type)
            .Select(c => new CostDto()
            {
                Type = c.Key,
                Total = c.Sum(i => ((NumberValue)_expressions[i.Type].Execute(
                    new ParameterCollection()
                    {
                        { "xp", i.XPosition },
                        { "yp", i.YPosition },
                        { "zp", i.ZPosition },
                        { "xl", i.XLength },
                        { "yl", i.YLength },
                        { "zl", i.ZLength },
                    })).Number)
            }).ToList();
        return costs;
    }

    public async Task<IEnumerable<FormulaDto>> ListFormulas()
    {
        using var repository = _factory.CreateReadRepository();
        return (await repository.ListAsync())
            .Select(x => new FormulaDto() { Id = x.Id, Type = x.Type, Expression = x.Expression });
    }

    public async Task<FormulaDto> UpdateFormula(FormulaDto dto)
    {
        using var repository = _factory.CreateRepository();
        var formula = await repository.GetByIdAsync(dto.Id);
        if (formula == null)
            throw new ArgumentException("Formula not exist. Consider using Add method instead.");

        // validate formula
        var processor = new Processor().Parse(formula.Expression);

        // persist into database
        formula.Type = dto.Type;
        formula.Expression = dto.Expression;
        await repository.UpdateAsync(formula);
        await repository.SaveChangesAsync();

        // update memory cache
        if (_expressions.ContainsKey(formula.Type))
            _expressions[formula.Type] = processor;
        else
            _expressions.Add(formula.Type, processor);

        return dto;
    }

    public async Task<FormulaDto> AddFormula(FormulaDto dto)
    {
        using var repository = _factory.CreateRepository();

        // validate formula
        var processor = new Processor().Parse(dto.Expression);

        // persist into database
        var formula = new Formula() { Type = dto.Type, Expression = dto.Expression };
        formula = await repository.AddAsync(formula);
        await repository.SaveChangesAsync();

        // update memory cache
        _expressions.Add(formula.Type, processor);

        return new FormulaDto() { Id = formula.Id, Type = formula.Type, Expression = formula.Expression };
    }

    public async Task DeleteFormulaById(int id)
    {
        using var repository = _factory.CreateRepository();

        var formula = repository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (formula == null) throw new ArgumentNullException();
        await repository.DeleteAsync(formula);
        await repository.SaveChangesAsync();
    }
}