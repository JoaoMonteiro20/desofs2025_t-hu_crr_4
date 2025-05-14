using EcoImpact.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HabitTypesController : ControllerBase
{
    private readonly IHabitTypeService _habitService;

    public HabitTypesController(IHabitTypeService habitService)
    {
        _habitService = habitService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HabitType>>> GetAll()
    {
        var habits = await _habitService.GetAllAsync();
        return Ok(habits);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HabitType>> GetById(Guid id)
    {
        var habit = await _habitService.GetByIdAsync(id);
        if (habit == null) return NotFound();
        return Ok(habit);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HabitType>> Create([FromBody] HabitTypeDto dto)
    {
        var created = await _habitService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.HabitTypeId }, created);
    }
}