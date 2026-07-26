using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Meetings.DTOs;
using TravelManagement.API.Features.Meetings.Interfaces;

namespace TravelManagement.API.Features.Meetings.Controllers;

[ApiController]
[Route("api/meetings")]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingsController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetings()
    {
        return Ok(await _meetingService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeeting(Guid id)
    {
        var meeting = await _meetingService.GetByIdAsync(id);

        if (meeting == null)
            return NotFound();

        return Ok(meeting);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeeting(CreateMeetingDto dto)
    {
        var meeting = await _meetingService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetMeeting),
            new { id = meeting.Id },
            meeting);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeeting(Guid id, UpdateMeetingDto dto)
    {
        var updated = await _meetingService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeeting(Guid id)
    {
        var deleted = await _meetingService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}