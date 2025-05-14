using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserChoicesController : ControllerBase
{
    private readonly EcoDbContext _context;

    public UserChoicesController(EcoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserChoice>>> GetUserChoices()
    {
        return await _context.UserChoices
            .Include(uc => uc.HabitType)
            .Include(uc => uc.User)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserChoice>> CreateUserChoice(UserChoice choice)
    {
        _context.UserChoices.Add(choice);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserChoices), new { id = choice.UserChoiceId }, choice);
    }
}
