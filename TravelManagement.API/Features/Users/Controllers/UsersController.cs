using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelManagement.API.Features.Users.DTOs;
using TravelManagement.API.Features.Users.Interfaces;

namespace TravelManagement.API.Features.Users.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(user);
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { message = "User not found." });

        return Ok(new
        {
            message = "User deleted successfully."
        });
    }
}