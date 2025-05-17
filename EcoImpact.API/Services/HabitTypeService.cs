using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class HabitTypeService : IHabitTypeService
{
    private readonly EcoDbContext _context;
    private readonly ILogger<HabitTypeService> _logger;

    public HabitTypeService(EcoDbContext context, ILogger<HabitTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<HabitType>> GetAllAsync()
    {
        _logger.LogInformation("Obtendo todos os HabitTypes");
        return await _context.HabitTypes.ToListAsync();
    }

    public async Task<HabitType?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando HabitType com ID: {Id}", id);
        var habit = await _context.HabitTypes.FindAsync(id);
        if (habit == null)
            _logger.LogWarning("HabitType com ID {Id} não encontrado", id);
        return habit;
    }

    public async Task<HabitType> CreateAsync(HabitTypeDto dto)
    {
        var habit = new HabitType
        {
            HabitTypeId = Guid.NewGuid(),
            Name = dto.Name,
            Unit = dto.Unit,
            Factor = dto.Factor
        };

        _logger.LogInformation("Criando HabitType: {Name}", dto.Name);

        _context.HabitTypes.Add(habit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType criado com sucesso. ID: {Id}", habit.HabitTypeId);

        return habit;
    }

    public async Task<HabitType?> UpdateAsync(Guid id, HabitTypeDto dto)
    {
        _logger.LogInformation("Atualizando HabitType com ID: {Id}", id);

        var existing = await _context.HabitTypes.FindAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado para atualização", id);
            return null;
        }

        existing.Name = dto.Name;
        existing.Unit = dto.Unit;
        existing.Factor = dto.Factor;

        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType com ID {Id} atualizado com sucesso", id);

        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Tentando eliminar HabitType com ID: {Id}", id);

        var habit = await _context.HabitTypes.FindAsync(id);
        if (habit == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado para eliminação", id);
            return false;
        }

        _context.HabitTypes.Remove(habit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType com ID {Id} eliminado com sucesso", id);
        return true;
    }
}
