using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Entities.DTOs;
using TravelManagement.API.Features.Entities.Interfaces;

namespace TravelManagement.API.Features.Entities.Controllers;

[ApiController]
[Route("api/entities")]
public class EntitiesController : ControllerBase
{
    private readonly IBusinessEntityService _service;

    public EntitiesController(IBusinessEntityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBusinessEntityDto dto)
    {
        var entity = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateBusinessEntityDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}