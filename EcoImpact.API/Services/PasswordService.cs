using Microsoft.AspNetCore.Identity;
using EcoImpact.DataModel.Models;
using EcoImpact.API.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string plainPassword)
    {
        return _hasher.HashPassword(user, plainPassword);
    }

    public bool VerifyPassword(User user, string hashedPassword, string inputPassword)
    {
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, inputPassword);
        return result == PasswordVerificationResult.Success;
    }
}
