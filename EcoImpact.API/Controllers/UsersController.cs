using Microsoft.AspNetCore.Mvc;
using EcoImpact.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using EcoImpact.DataModel.Dtos;
using EcoImpact.API.Services;

namespace EcoImpact.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    [AllowAnonymous] // Remova se não quiser permitir criação sem login
    public async Task<ActionResult<User>> CreateUser(CreateUserDto user)
    {
        var created = await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto updatedUser)
    {
        var updated = await _userService.UpdateAsync(id, updatedUser);
        if (updated == null) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await _userService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("export-json")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> ExportUsersJson()
    {
        var result = await _userService.ExportUsersAsJsonFileAsync();
        return File(result.FileContent, result.ContentType, result.FileName);
    }
}
