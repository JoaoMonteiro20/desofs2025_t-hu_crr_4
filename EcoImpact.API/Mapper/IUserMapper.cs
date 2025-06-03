using EcoImpact.DataModel.Dtos;
using EcoImpact.DataModel.Models;

namespace EcoImpact.API.Mapper
{
    public interface IUserMapper
    {
        void UpdateUserFromDto(User user, UserUpdateDto dto);
    }
}
