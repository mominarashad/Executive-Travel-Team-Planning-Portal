using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Hotels.DTOs;
using TravelManagement.API.Features.Hotels.Interfaces;

namespace TravelManagement.API.Features.Hotels.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _hotelService.GetAllAsync();
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        return hotel == null ? NotFound() : Ok(hotel);
    }

    [HttpGet("city/{cityId}")]
    public async Task<IActionResult> GetByCity(Guid cityId)
    {
        var hotels = await _hotelService.GetByCityAsync(cityId);
        return Ok(hotels);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelDto dto)
    {
        var hotel = await _hotelService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateHotelDto dto)
    {
        var updated = await _hotelService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _hotelService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}