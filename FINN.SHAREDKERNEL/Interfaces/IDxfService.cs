using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Dtos;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IDxfService
{
    /// <summary>
    ///     Draw dxf file based on layout items.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    string DrawLayout(LayoutDto dto);

    /// <summary>
    ///     Read geometry objects from dxf file.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    IEnumerable<GeometryDto> ReadLayout(string filename);

    /// <summary>
    ///     Get the block definition record from database by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BlockDefinitionDto?> GetBlockDefinition(int id);

    /// <summary>
    ///     Add block definition to database by block names. If blockNames is null, all blocks inside the file will be added.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="blockNames"></param>
    /// <returns></returns>
    Task<IEnumerable<BlockDefinitionDto>> AddBlockDefinitions(string filename, IEnumerable<string>? blockNames);

    /// <summary>
    ///     Delete a block definition from database by Id.
    /// </summary>
    /// <param name="id"></param>
    Task DeleteBlockDefinitionById(int id);

    /// <summary>
    ///     List a page of the block definitions.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<IEnumerable<BlockDefinitionDto>> ListBlockDefinitions(PaginationFilter filter);

    /// <summary>
    ///     Prepare a downloaded file in tmp folder.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<string> DownloadBlockFile(int id);
}