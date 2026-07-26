using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.TeamPlans.DTOs;
using TravelManagement.API.Features.TeamPlans.Interfaces;

namespace TravelManagement.API.Features.TeamPlans.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamPlansController : ControllerBase
{
    private readonly ITeamPlanService _service;

    public TeamPlansController(ITeamPlanService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamPlanDto>>> GetAll()
    {
        var plans = await _service.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("summary/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<TeamPlanSummaryDto>>> GetSummary(Guid userId)
    {
        var result = await _service.GetSummaryAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamPlanDto>> GetById(Guid id)
    {
        var plan = await _service.GetByIdAsync(id);

        if (plan == null)
            return NotFound();

        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<TeamPlanDto>> Create(CreateTeamPlanDto dto)
    {
        var plan = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = plan.Id },
            plan);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamPlanDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate(BulkCreateTeamPlanDto dto)
    {
        await _service.BulkCreateAsync(dto);
        return Ok(new
        {
            message = "Team plans created successfully."
        });
    }
}