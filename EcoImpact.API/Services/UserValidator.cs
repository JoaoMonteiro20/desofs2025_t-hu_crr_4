using EcoImpact.DataModel.Dtos;
using System.Text.RegularExpressions;

namespace EcoImpact.API.Services
{
    public class UserValidator : IUserValidator
    {
        public void Validate(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                throw new ArgumentException("A valid email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");

            ValidatePassword(dto.Password);
        }

        public void Validate(UserUpdateDto dto)
        {
            if (dto.UserName != null && string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("Username cannot be empty.");

            if (dto.Email != null && !IsValidEmail(dto.Email))
                throw new ArgumentException("A valid email is required.");

            if (!string.IsNullOrWhiteSpace(dto.Password))
                ValidatePassword(dto.Password);
        }

        private bool IsValidEmail(string email)
        {
            var pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private void ValidatePassword(string password)
        {
            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            if (!password.Any(char.IsDigit))
                throw new ArgumentException("Password must contain at least one digit.");

            if (!password.Any(char.IsLower))
                throw new ArgumentException("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsUpper))
                throw new ArgumentException("Password must contain at least one uppercase letter.");

            if (!password.Any(ch => "!@#$%^&*()-_=+[]{}|;:,.<>?/\\`~".Contains(ch)))
                throw new ArgumentException("Password must contain at least one special character.");
        }
    }
}
