using Microsoft.AspNetCore.Mvc;
using System.Text;
using TravelManagement.API.Features.Email.Interfaces;
using TravelManagement.API.Features.OnePager.DTOs;
using TravelManagement.API.Features.OnePager.Interfaces;

namespace TravelManagement.API.Features.OnePager.Controllers;

public class SendOnePagerRequest
{
    public string ToEmail { get; set; } = string.Empty;
}

[ApiController]
[Route("api/onepager")]
public class OnePagerController : ControllerBase
{
    private readonly IOnePagerService _service;
    private readonly IEmailService _emailService;

    public OnePagerController(IOnePagerService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOnePager(Guid userId)
    {
        var onePager = await _service.GetOnePagerAsync(userId);

        if (onePager == null)
            return NotFound();

        return Ok(onePager);
    }

    [HttpPost("{userId}/send")]
    public async Task<IActionResult> SendOnePager(Guid userId, [FromBody] SendOnePagerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ToEmail))
            return BadRequest(new { message = "Recipient email is required." });

        var onePager = await _service.GetOnePagerAsync(userId);
        if (onePager == null)
            return NotFound();

        var html = BuildOnePagerHtml(onePager);
        await _emailService.SendEmailAsync(
            request.ToEmail,
            $"Travel Brief — {onePager.Name}",
            html);

        return Ok(new { message = $"One-pager emailed to {request.ToEmail}." });
    }

    private static string BuildOnePagerHtml(OnePagerDto data)
    {
        var sb = new StringBuilder();
        sb.Append($"<h2>{data.Name}</h2><p>{data.Title} · {data.Function}</p><hr/>");

        sb.Append("<h3>Itinerary</h3><table border='1' cellpadding='6' style='border-collapse:collapse;width:100%'>");
        sb.Append("<tr><th>City</th><th>From</th><th>To</th><th>Type</th></tr>");
        foreach (var entry in data.Itinerary)
        {
            sb.Append($"<tr><td>{entry.CityName}</td><td>{entry.FromDate}</td><td>{entry.ToDate}</td><td>{entry.Type}</td></tr>");
        }
        sb.Append("</table>");

        sb.Append("<h3>Days by Country</h3><ul>");
        foreach (var d in data.DaysByCountry)
        {
            sb.Append($"<li>{d.Country}: {d.Days} days</li>");
        }
        sb.Append($"</ul><p><strong>Total: {data.TotalDays} days</strong></p>");

        if (data.Flights.Count > 0)
        {
            sb.Append("<h3>Flights</h3><table border='1' cellpadding='6' style='border-collapse:collapse;width:100%'>");
            sb.Append("<tr><th>Airline</th><th>Flight</th><th>Route</th><th>Depart</th></tr>");
            foreach (var f in data.Flights)
            {
                sb.Append($"<tr><td>{f.Airline}</td><td>{f.FlightNumber}</td><td>{f.DepartureAirport} → {f.ArrivalAirport}</td><td>{f.DepartureTime:yyyy-MM-dd HH:mm}</td></tr>");
            }
            sb.Append("</table>");
        }

        sb.Append("<h3>Meetings</h3>");
        foreach (var m in data.Meetings)
        {
            sb.Append($"<p><strong>#{m.DisplayOrder} — {m.ContactName}</strong> ({m.TripCity}, {m.TripStartDate} to {m.TripEndDate})<br/>");
            sb.Append($"{m.Priority} priority · {m.Status}<br/>{m.Agenda}</p>");
        }

        return sb.ToString();
    }
}
