using Microsoft.AspNetCore.Mvc;
using EcoImpact.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using EcoImpact.DataModel.Dtos;
using EcoImpact.API.Services;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();

        var dtoList = users.Select(u => new UserDto
        {
            UserName = u.UserName,
            Email = u.Email,
            Role = u.Role.ToString()
        });

        return Ok(dtoList);
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

    [HttpPost("quiz/save-score")]
    public async Task<IActionResult> SaveEcoScore([FromBody] SaveEcoScoreDto dto)
    {
        var siid = User.Identity?.Name;
        if (string.IsNullOrEmpty(siid)) return Unauthorized();

        var sucesso = await _userService.UpdateEcoScoreAsync(siid, dto.Score);
        if (!sucesso) return NotFound();

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
  
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized();

        var user = await _userService.GetByUsernameAsync(username);

        if (user == null)
            return Unauthorized();

        return Ok(new UserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString(),
            EcoScore = user.EcoScore
        });
    }
}
