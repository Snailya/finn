using System.Data;
using FINN.SHAREDKERNEL.Dtos;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IExcelService
{
    (IEnumerable<GridDto>, IEnumerable<PlatformDto>) ReadAsGridSheet(DataTable dataTable);
    IEnumerable<ProcessDto> ReadAsProcessListSheet(DataTable dataTable);
}