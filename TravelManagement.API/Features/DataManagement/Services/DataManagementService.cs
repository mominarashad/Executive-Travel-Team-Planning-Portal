using TravelManagement.API.Features.DataManagement.DTOs;
using TravelManagement.API.Features.DataManagement.Interfaces;

namespace TravelManagement.API.Features.DataManagement.Services;

public class DataManagementService : IDataManagementService
{
    private readonly IDataManagementRepository _repository;

    public DataManagementService(IDataManagementRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExportDataDto> ExportAsync() => await _repository.ExportAsync();

    public async Task ImportAsync(ExportDataDto data) => await _repository.ImportAsync(data);
}