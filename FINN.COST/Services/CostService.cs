using FINN.CORE.Interfaces;
using FINN.COST.Models;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.Extensions.Logging;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;

namespace FINN.COST.Services;

public class CostService
{
    private readonly ILogger<CostService> _logger;
    private readonly IRepository<Formula> _repository;
    private readonly Dictionary<string, IExpression> _expressions = new();

    public CostService(ILogger<CostService> logger, IRepository<Formula> repository)
    {
        _logger = logger;
        _repository = repository;

        // read config
        // _expressions.Add("platform",
        //     new Processor().Parse("if(zp>5000,130,120)*1.2E-9*xl*yl"));

        Initialize();
    }

    private void Initialize()
    {
        var formulas = _repository.ListAsync().GetAwaiter().GetResult();
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

    public IEnumerable<FormulaDto> ListFormulas()
    {
        return _repository.ListAsync().GetAwaiter().GetResult()
            .Select(x => new FormulaDto() { Id = x.Id, Type = x.Type, Expression = x.Expression });
    }

    public FormulaDto UpdateFormula(FormulaDto dto)
    {
        var formula = _repository.GetByIdAsync(dto.Id).GetAwaiter().GetResult();
        if (formula == null)
            throw new ArgumentException("Formula not exist. Consider using Add method instead.");

        // validate formula
        var processor = new Processor().Parse(formula.Expression);

        // persist into database
        formula.Type = dto.Type;
        formula.Expression = dto.Expression;
        _repository.UpdateAsync(formula).Wait();

        // update memory cache
        if (_expressions.ContainsKey(formula.Type))
            _expressions[formula.Type] = processor;
        else
            _expressions.Add(formula.Type, processor);

        return dto;
    }

    public FormulaDto AddFormula(FormulaDto dto)
    {
        // validate formula
        var processor = new Processor().Parse(dto.Expression);

        // persist into database
        var formula = new Formula() { Type = dto.Type, Expression = dto.Expression };
        _repository.AddAsync(formula).GetAwaiter().GetResult();

        // update memory cache
        _expressions.Add(formula.Type, processor);

        return new FormulaDto() { Id = formula.Id, Type = formula.Type, Expression = formula.Expression };
    }

    public void DeleteFormulaById(int id)
    {
        var formula = _repository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (formula == null) throw new ArgumentNullException();
        _repository.DeleteAsync(formula).Wait();
    }
}