using EcoImpact.DataModel.Models;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<bool> DeleteAsync(Guid id);

    Task<bool> UpdateAsync(Guid id, User updatedUser);
    Task<UserFileExportResult> ExportUsersAsJsonFileAsync();
}