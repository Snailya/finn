using System.Data;
using FINN.SHAREDKERNEL.Dtos.Drafter;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IDataTableReader
{
    (IEnumerable<GridDto>, IEnumerable<PlatformBlockDto>) ReadAsGridSheet(DataTable dataTable);
    IEnumerable<ProcessDto> ReadAsProcessListSheet(DataTable dataTable);
}