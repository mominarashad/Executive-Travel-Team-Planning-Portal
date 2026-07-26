using TravelManagement.API.Features.Dashboard.DTOs;
using TravelManagement.API.Features.Dashboard.Interfaces;

namespace TravelManagement.API.Features.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardDto> GetDashboardAsync() =>
        await _repository.GetDashboardAsync();
}