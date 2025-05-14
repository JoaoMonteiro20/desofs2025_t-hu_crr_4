using EcoImpact.DataModel.Dtos;
using EcoImpact.DataModel.Models;

public interface IUserChoiceService
{
    Task<IEnumerable<UserChoice>> GetAllAsync();
    Task<UserChoice?> GetByIdAsync(Guid id);
    Task<UserChoice> CreateAsync(UserChoiceDto dto);
}