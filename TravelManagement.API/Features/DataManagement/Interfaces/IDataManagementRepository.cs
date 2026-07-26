using TravelManagement.API.Features.DataManagement.DTOs;

namespace TravelManagement.API.Features.DataManagement.Interfaces;

public interface IDataManagementRepository
{
    Task<ExportDataDto> ExportAsync();
    Task ImportAsync(ExportDataDto data);
}