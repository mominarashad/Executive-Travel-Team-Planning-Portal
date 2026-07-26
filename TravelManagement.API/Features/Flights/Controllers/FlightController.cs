using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Flights.DTOs;
using TravelManagement.API.Features.Flights.Interfaces;

namespace TravelManagement.API.Features.Flights.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFlights()
    {
        var flights = await _flightService.GetAllAsync();
        return Ok(flights);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFlight(Guid id)
    {
        var flight = await _flightService.GetByIdAsync(id);

        if (flight == null)
            return NotFound();

        return Ok(flight);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlight(CreateFlightDto dto)
    {
        var flight = await _flightService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetFlight),
            new { id = flight.Id },
            flight);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlight(Guid id, UpdateFlightDto dto)
    {
        var updated = await _flightService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlight(Guid id)
    {
        var deleted = await _flightService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}