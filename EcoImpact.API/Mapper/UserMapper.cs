using EcoImpact.DataModel.Dtos;
using EcoImpact.DataModel.Models;

namespace EcoImpact.API.Mapper
{
    public class UserMapper : IUserMapper
    {
        public void UpdateUserFromDto(User user, UserUpdateDto dto)
        {
            if (dto.UserName != null)
                user.UserName = dto.UserName;

            if (dto.Email != null)
                user.Email = dto.Email;

        }
    }
}
