using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Auth.DTOs;
using TravelManagement.API.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace TravelManagement.API.Features.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(Guid.Parse(userId));

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();

        return Ok(new
        {
            message = "Logged out successfully."
        });
    }
}