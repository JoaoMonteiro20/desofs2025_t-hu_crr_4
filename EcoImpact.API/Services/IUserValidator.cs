using EcoImpact.DataModel.Dtos;

namespace EcoImpact.API.Services
{
    public interface IUserValidator
    {
        void Validate(CreateUserDto dto);
        void Validate(UserUpdateDto dto);
    }
}
