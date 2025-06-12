using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoImpact.DataModel;
using EcoImpact.DataModel.Models;
using EcoImpact.API.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly EcoDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        EcoDbContext context,
        IPasswordService passwordService,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        _logger.LogInformation("Iniciando autenticação para utilizador: {Username}", username);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
        {
            _logger.LogWarning("Utilizador não encontrado: {Username}", username);
            return null;
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("Utilizador {Username} está temporariamente bloqueado até {LockoutEnd}", username, user.LockoutEnd);
            throw new InvalidOperationException($"Utilizador bloqueado temporariamente até {user.LockoutEnd?.ToLocalTime():HH:mm:ss dd/MM/yyyy}.");
        }

        if (!_passwordService.VerifyPassword(user, user.Password, password))
        {
            user.FailedLoginAttempts++;

            _logger.LogWarning("Password incorreta para utilizador: {Username}. Tentativa {Count}/4", username, user.FailedLoginAttempts);

            if (user.FailedLoginAttempts >= 4)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);
                _logger.LogWarning("Utilizador {Username} foi bloqueado até {LockoutEnd}", username, user.LockoutEnd);
            }

            await _context.SaveChangesAsync();
            return null;
        }

        _logger.LogInformation("Autenticação bem-sucedida para utilizador: {Username}", user.UserName);

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await _context.SaveChangesAsync();

        // Gerar JWT
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var jwtSecret = Environment.GetEnvironmentVariable("JWT")?.Trim()
        ?? throw new Exception("JWT não definido nas variáveis de ambiente.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        _logger.LogInformation("Token JWT gerado para utilizador: {Username}", user.UserName);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
