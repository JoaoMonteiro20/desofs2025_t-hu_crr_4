using EcoImpact.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class HabitTypesController : ControllerBase
{
    private readonly IHabitTypeService _habitService;
    private readonly ILogger<HabitTypesController> _logger;

    public HabitTypesController(IHabitTypeService habitService, ILogger<HabitTypesController> logger)
    {
        _habitService = habitService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HabitType>>> GetAll()
    {
        _logger.LogInformation("Pedido para listar todos os HabitTypes");
        var habits = await _habitService.GetAllAsync();
        return Ok(habits);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HabitType>> GetById(Guid id)
    {
        _logger.LogInformation("Pedido para obter HabitType com ID: {Id}", id);
        var habit = await _habitService.GetByIdAsync(id);
        if (habit == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado", id);
            return NotFound();
        }

        return Ok(habit);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<ActionResult<HabitType>> Create([FromBody] HabitTypeDto dto)
    {
        _logger.LogInformation("Utilizador {User} a criar HabitType: {Name}", User.Identity?.Name, dto.Name);
        var created = await _habitService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.HabitTypeId }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<ActionResult<HabitType>> Update(Guid id, [FromBody] HabitTypeDto dto)
    {
        _logger.LogInformation("Utilizador {User} a atualizar HabitType ID: {Id}", User.Identity?.Name, id);
        var updated = await _habitService.UpdateAsync(id, dto);
        if (updated == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado para atualização", id);
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Utilizador {User} a tentar eliminar HabitType ID: {Id}", User.Identity?.Name, id);
        var success = await _habitService.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("Tentativa de eliminar HabitType não encontrado: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("HabitType com ID {Id} eliminado com sucesso", id);
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> ImportHabitTypes(IFormFile file)
    {
        var result = await _habitService.ImportFromFileAsync(file);
        return Ok(result);
    }
}
