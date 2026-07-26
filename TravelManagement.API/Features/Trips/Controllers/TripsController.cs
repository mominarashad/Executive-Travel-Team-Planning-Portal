using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Trips.DTOs;
using TravelManagement.API.Features.Trips.Interfaces;

namespace TravelManagement.API.Features.Trips.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _tripService.GetAllAsync();
        return Ok(trips);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTrips(
        Guid? cityId,
        Guid? projectId,
        Guid? personId,
        string? search)
    {
        var result = await _tripService.SearchAsync(
            cityId,
            projectId,
            personId,
            search);

        return Ok(result);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate(BulkCreateTripDto dto)
    {
        await _tripService.BulkCreateAsync(dto);

        return Ok(new
        {
            message = "Trips created successfully."
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrip(Guid id)
    {
        var trip = await _tripService.GetByIdAsync(id);

        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrip(CreateTripDto dto)
    {
        var trip = await _tripService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetTrip),
            new { id = trip.Id },
            trip);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTrip(Guid id, UpdateTripDto dto)
    {
        var updated = await _tripService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(Guid id)
    {
        var deleted = await _tripService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}