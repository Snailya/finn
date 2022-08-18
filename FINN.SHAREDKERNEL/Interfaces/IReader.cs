using System.Data;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IReader
{
    (IEnumerable<GridDto>, IEnumerable<PlateDto>) ReadAsGridSheet(DataTable dataTable);
    IEnumerable<ProcessDto> ReadAsProcessListSheet(DataTable dataTable);
}