using EcoImpact.DataModel.Models;

public interface IHabitTypeService
{
    Task<IEnumerable<HabitTypeDto>> GetAllAsync();
    Task<HabitTypeDto?> GetByIdAsync(Guid id);
    Task<HabitType> CreateAsync(HabitTypeDto dto);

    Task<HabitTypeDto> UpdateAsync(Guid id,HabitTypeDto dto);

    Task<bool> DeleteAsync(Guid id);
    Task<string> ImportFromFileAsync(IFormFile file);
}