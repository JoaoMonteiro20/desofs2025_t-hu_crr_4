using EcoImpact.DataModel.Models;

public interface IHabitTypeService
{
    Task<IEnumerable<HabitType>> GetAllAsync();
    Task<HabitType?> GetByIdAsync(Guid id);
    Task<HabitType> CreateAsync(HabitTypeDto dto);
}