using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class HabitTypesController : ControllerBase
{
    private readonly EcoDbContext _context;

    public HabitTypesController(EcoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HabitType>>> GetHabitTypes()
    {
        return await _context.HabitTypes.ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HabitType>> CreateHabitType(HabitType habit)
    {
        _context.HabitTypes.Add(habit);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetHabitTypes), new { id = habit.HabitTypeId }, habit);
    }
}
