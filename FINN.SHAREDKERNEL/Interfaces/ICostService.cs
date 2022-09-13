using FINN.SHAREDKERNEL.Dtos;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface ICostService
{
    /// <summary>
    /// Estimate the cost of each categories from geomtry.
    /// </summary>
    IEnumerable<CostDto> EstimateCost(IEnumerable<GeometryDto> dto);

    /// <summary>
    /// List all formulas in the database.
    /// </summary>
    Task<IEnumerable<FormulaDto>> ListFormulas();

    /// <summary>
    /// Update the expression string of the formula.
    /// </summary>
    Task<FormulaDto> UpdateFormula(FormulaDto dto);

    /// <summary>
    /// Add a new calculate formula for specified category.
    /// </summary>
    Task<FormulaDto> AddFormula(FormulaDto dto);

    /// <summary>
    /// Delete a formula from database.
    /// </summary>
    Task DeleteFormulaById(int id);
}