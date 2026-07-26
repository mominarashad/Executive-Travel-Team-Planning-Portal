using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.OnePager.Interfaces;

namespace TravelManagement.API.Features.OnePager.Controllers;

[ApiController]
[Route("api/onepager")]
public class OnePagerController : ControllerBase
{
    private readonly IOnePagerService _service;

    public OnePagerController(IOnePagerService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOnePager(Guid userId)
    {
        var onePager = await _service.GetOnePagerAsync(userId);

        if (onePager == null)
            return NotFound();

        return Ok(onePager);
    }
}