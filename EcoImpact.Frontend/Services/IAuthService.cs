using EcoImpact.Frontend.Models;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task LogoutAsync();

    Task<AuthResult> RegisterAsync(RegisterRequest request);
}