using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Directory.DTOs;
using TravelManagement.API.Features.Directory.Interfaces;

namespace TravelManagement.API.Features.Directory.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly IContactService _contactService;

    public DirectoryController(
        ICityService cityService,
        IContactService contactService)
    {
        _cityService = cityService;
        _contactService = contactService;
    }
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _cityService.GetAllAsync();
        return Ok(cities);
    }

    [HttpGet("cities/{id}")]
    public async Task<IActionResult> GetCity(Guid id)
    {
        var city = await _cityService.GetByIdAsync(id);

        if (city == null)
            return NotFound();

        return Ok(city);
    }

    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity(CreateCityDto dto)
    {
        var city = await _cityService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetCity),
            new { id = city.Id },
            city
        );
    }

    [HttpGet("cities/{cityId}/contacts")]
    public async Task<IActionResult> GetContactsByCity(Guid cityId)
    {
        var contacts = await _contactService.GetByCityAsync(cityId);

        return Ok(contacts);
    }

    [HttpGet("cities/autocomplete")]
    public async Task<IActionResult> AutocompleteCities([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("Search term is required.");

        var cities = await _cityService.AutocompleteAsync(term);

        return Ok(cities);
    }

    [HttpPut("cities/{id}")]
    public async Task<IActionResult> UpdateCity(Guid id, UpdateCityDto dto)
    {
        var updated = await _cityService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("cities/{id}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var deleted = await _cityService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("contacts")]
    public async Task<IActionResult> GetContacts()
    {
        var contacts = await _contactService.GetAllAsync();
        return Ok(contacts);
    }

    [HttpGet("contacts/{id}")]
    public async Task<IActionResult> GetContact(Guid id)
    {
        var contact = await _contactService.GetByIdAsync(id);

        if (contact == null)
            return NotFound();

        return Ok(contact);
    }

    [HttpPost("contacts")]
    public async Task<IActionResult> CreateContact(CreateContactDto dto)
    {
        var contact = await _contactService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetContact),
            new { id = contact.Id },
            contact);
    }

    [HttpPut("contacts/{id}")]
    public async Task<IActionResult> UpdateContact(Guid id, UpdateContactDto dto)
    {
        var updated = await _contactService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("contacts/{id}")]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        var deleted = await _contactService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}