using System.Data;
using FINN.SHAREDKERNEL.Dtos;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IReader
{
    IEnumerable<GridDto> ReadAsGrid(DataTable dataTable);
    IEnumerable<ProcessDto> ReadAsProcessList(DataTable dataTable);
}