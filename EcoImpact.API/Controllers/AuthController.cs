using Microsoft.AspNetCore.Mvc;
using EcoImpact.DataModel.Dtos;
using EcoImpact.API.Services;

namespace EcoImpact.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var token = await _authService.AuthenticateAsync(request.UserName, request.Password);

        if (token == null)
            return Unauthorized("Credenciais inválidas");

        return Ok(new { token });
    }
}