using EcoImpact.DataModel.Dtos;
using EcoImpact.DataModel.Models;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(CreateUserDto user);
    Task<bool> DeleteAsync(Guid id);

    Task<User?> UpdateAsync(Guid id, UserUpdateDto dto);
    Task<UserFileExportResult> ExportUsersAsJsonFileAsync();
    Task<bool> UpdateEcoScoreAsync(string username, decimal score);
}