using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Dashboard.Interfaces;

namespace TravelManagement.API.Features.Dashboard.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _service.GetDashboardAsync();
        return Ok(dashboard);
    }
}