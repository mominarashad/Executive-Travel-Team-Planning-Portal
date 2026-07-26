using TravelManagement.API.Features.Dashboard.DTOs;

namespace TravelManagement.API.Features.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}