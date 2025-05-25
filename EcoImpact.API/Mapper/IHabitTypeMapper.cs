using EcoImpact.DataModel.Models;

namespace EcoImpact.API.Mapper
{
    public interface IHabitTypeMapper
    {
        HabitType ToEntity(HabitTypeDto dto);
        HabitTypeDto ToDto(HabitType entity);
    }
}
