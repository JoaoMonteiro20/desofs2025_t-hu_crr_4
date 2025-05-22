using EcoImpact.API.Mapper;
using EcoImpact.DataModel.Models;

public class HabitTypeMapper : IHabitTypeMapper
{
    public HabitType ToEntity(HabitTypeDto dto)
    {
        return new HabitType
        {
            HabitTypeId = Guid.NewGuid(),
            Name = dto.Name,
            Factor = dto.Factor,
            Unit = dto.Unit
        };
    }

    public HabitTypeDto ToDto(HabitType entity)
    {
        return new HabitTypeDto
        {
            Name = entity.Name,
            Factor = entity.Factor,
            Unit = entity.Unit
        };
    }
}