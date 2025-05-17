using EcoImpact.DataModel.Models;

namespace EcoImpact.API.Services
{
    public interface IPasswordService
    {
        string HashPassword(User user, string plainPassword);
        bool VerifyPassword(User user, string hashedPassword, string inputPassword);
    }
}
