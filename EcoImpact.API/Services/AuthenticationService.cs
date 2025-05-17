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

        if (!_passwordService.VerifyPassword(user, user.Password, password))
        {
            _logger.LogWarning("Password incorreta para utilizador: {Username}", username);
            return null;
        }

        _logger.LogInformation("Autenticação bem-sucedida para utilizador: {Username} com role: {Role}", user.UserName, user.Role);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        _logger.LogInformation("Token JWT gerado para utilizador: {Username}", user.UserName);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
