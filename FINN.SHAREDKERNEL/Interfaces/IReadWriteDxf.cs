using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Dtos.Cost;
using FINN.SHAREDKERNEL.Dtos.Drafter;
using FINN.SHAREDKERNEL.Dtos.Management;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IReadWriteDxf
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    string DrawLayout(DrawLayoutRequestDto requestDto);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    EstimateCostRequestDto EstimateFromFile(string filename);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    IEnumerable<BlockDefinition> InsertBlockDefinitions(InsertBlockRequestDto? requestDto);
}