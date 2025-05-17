using Microsoft.AspNetCore.Mvc;
using EcoImpact.DataModel.Dtos;
using EcoImpact.API.Services;
using Microsoft.Extensions.Logging;

namespace EcoImpact.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation("Tentativa de login para utilizador: {Username}", request.UserName);

        var token = await _authService.AuthenticateAsync(request.UserName, request.Password);

        if (token == null)
        {
            _logger.LogWarning("Login falhado para utilizador: {Username}", request.UserName);
            return Unauthorized("Credenciais inválidas");
        }

        _logger.LogInformation("Login bem-sucedido para utilizador: {Username}", request.UserName);
        return Ok(new { token });
    }
}