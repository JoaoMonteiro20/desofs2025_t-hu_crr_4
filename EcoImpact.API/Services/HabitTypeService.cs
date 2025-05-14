using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;

public class HabitTypeService : IHabitTypeService
{
    private readonly EcoDbContext _context;

    public HabitTypeService(EcoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HabitType>> GetAllAsync() =>
        await _context.HabitTypes.ToListAsync();

    public async Task<HabitType?> GetByIdAsync(Guid id) =>
        await _context.HabitTypes.FindAsync(id);

    public async Task<HabitType> CreateAsync(HabitTypeDto dto)
    {
        var habit = new HabitType { Name = dto.HabitName };
        _context.HabitTypes.Add(habit);
        await _context.SaveChangesAsync();
        return habit;
    }
}