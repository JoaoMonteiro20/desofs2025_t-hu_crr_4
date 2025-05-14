using EcoImpact.API.Services;
using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly EcoDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;

    public UserService(EcoDbContext context, IPasswordService passwordService, ILogger<UserService> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<User> CreateAsync(User user)
    {
        _logger.LogInformation("Criando utilizador com username: {UserName}", user.UserName);

        user.Password = _passwordService.HashPassword(user, user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilizador criado com sucesso: {UserId}", user.UserId);
        return user;
    }

    public async Task<bool> UpdateAsync(Guid id, User updatedUser)
    {
        if (id != updatedUser.UserId)
        {
            _logger.LogWarning("Tentativa de atualizar utilizador com ID divergente: {PathId} != {BodyId}", id, updatedUser.UserId);
            return false;
        }

        _logger.LogInformation("Atualizando utilizador com ID: {UserId}", id);

        _context.Entry(updatedUser).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilizador com ID {UserId} atualizado com sucesso", id);
        return true;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        _logger.LogInformation("Obtendo todos os utilizadores");
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando utilizador com ID: {UserId}", id);
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            _logger.LogWarning("Utilizador com ID {UserId} não encontrado", id);

        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Tentando eliminar utilizador com ID: {UserId}", id);

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Utilizador com ID {UserId} não encontrado para eliminação", id);
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilizador com ID {UserId} eliminado com sucesso", id);
        return true;
    }
}
