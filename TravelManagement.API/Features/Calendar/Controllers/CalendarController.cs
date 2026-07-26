using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Calendar.Interfaces;

namespace TravelManagement.API.Features.Calendar.Controllers;

[ApiController]
[Route("api/calendar")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _service;

    public CalendarController(ICalendarService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalendar(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] List<Guid>? personIds)
    {
        if (to < from)
            return BadRequest("'to' must be on or after 'from'.");

        var calendar = await _service.GetCalendarAsync(from, to, personIds);
        return Ok(calendar);
    }
}