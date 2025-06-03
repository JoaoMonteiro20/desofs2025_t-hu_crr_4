using EcoImpact.API.Services;
using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EcoImpact.DataModel.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using EcoImpact.API.Mapper;

public class UserService : IUserService
{
    private readonly EcoDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IUserValidator _validator;
    private readonly IUserMapper _mapper;

    public UserService(
        EcoDbContext context,
        IPasswordService passwordService,
        ILogger<UserService> logger,
        JsonSerializerOptions jsonOptions,
        IUserValidator validator,
        IUserMapper mapper
        )
    {
        _context = context;
        _passwordService = passwordService;
        _logger = logger;
        _jsonOptions = jsonOptions;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<User> CreateAsync(CreateUserDto dto)
    {
        _logger.LogInformation("Creating user with username: {UserName}", dto.UserName);

        _validator.Validate(dto);

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Role = UserRole.User,
            Password = _passwordService.HashPassword(null!, dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created with ID: {UserId}", user.UserId);
        return user;
    }

    public async Task<User?> UpdateAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", userId);
            return null;
        }

        _validator.Validate(dto);
        _mapper.UpdateUserFromDto(user, dto);

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.Password = _passwordService.HashPassword(user, dto.Password);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User with ID {UserId} updated successfully.", userId);
        return user;
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

    public async Task<UserFileExportResult> ExportUsersAsJsonFileAsync()
    {
        var users = await _context.Users
            .Select(u => new UserExportDto
            {
                Id = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Role = (int)u.Role
            })
            .ToListAsync();

        if (users.Count == 0)
            throw new InvalidOperationException("Não existem utilizadores para exportar.");

        var json = JsonSerializer.Serialize(users, _jsonOptions);
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        return new UserFileExportResult
        {
            FileContent = jsonBytes,
            ContentType = "application/json",
            FileName = "users_export.json"
        };
    }
}
