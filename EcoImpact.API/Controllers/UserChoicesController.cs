using EcoImpact.DataModel.Models;
using EcoImpact.DataModel.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class UserChoicesController : ControllerBase
{
    private readonly IUserChoiceService _service;
    private readonly ILogger<UserChoicesController> _logger;

    public UserChoicesController(IUserChoiceService service, ILogger<UserChoicesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserChoice>>> GetAll()
    {
        var choices = await _service.GetAllAsync();
        return Ok(choices);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserChoice>> GetById(Guid id)
    {
        var choice = await _service.GetByIdAsync(id);
        if (choice == null) return NotFound();
        return Ok(choice);
    }

    [HttpPost]
    public async Task<ActionResult<UserChoice>> Create([FromBody] UserChoiceDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.UserChoiceId }, created);
    }
}
