using FINN.SHAREDKERNEL.Dtos;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IDxfService
{
    /// <summary>
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    string DrawLayout(LayoutDto dto);

    /// <summary>
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    IEnumerable<GeometryDto> ReadLayout(string filename);

    BlockDefinitionDto? GetBlockDefinition(int id);
    IEnumerable<BlockDefinitionDto> AddBlockDefinitions(string filename, IEnumerable<string>? blockNames);
    void DeleteBlockDefinitionById(int id);
}