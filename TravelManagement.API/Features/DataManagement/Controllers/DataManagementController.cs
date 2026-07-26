using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.DataManagement.DTOs;
using TravelManagement.API.Features.DataManagement.Interfaces;

namespace TravelManagement.API.Features.DataManagement.Controllers;

[ApiController]
[Route("api/data")]
public class DataManagementController : ControllerBase
{
    private readonly IDataManagementService _service;

    public DataManagementController(IDataManagementService service)
    {
        _service = service;
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _service.ExportAsync();

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var bytes = Encoding.UTF8.GetBytes(json);
        var fileName = $"travelmanagement-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";

        return File(bytes, "application/json", fileName);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ExportDataDto data)
    {
        await _service.ImportAsync(data);

        return Ok(new
        {
            message = "Data imported successfully. User accounts were not modified."
        });
    }
}