using EcoImpact.DataModel.Models;
using EcoImpact.API.Mapper;

namespace EcoImpact.Tests
{
    [TestClass]
    public class HabitTypeMapperTests
    {
        private HabitTypeMapper _mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            _mapper = new HabitTypeMapper();
        }

        [TestMethod]
        public void ToEntity_ShouldMapAllPropertiesFromDto()
        {
            // Arrange
            var dto = new HabitTypeDto
            {
                Name = "Energia",
                Unit = "kWh",
                Factor = 0.15M
            };

            // Act
            var entity = _mapper.ToEntity(dto);

            // Assert
            Assert.AreEqual(dto.Name, entity.Name);
            Assert.AreEqual(dto.Unit, entity.Unit);
            Assert.AreEqual(dto.Factor, entity.Factor);
            Assert.AreNotEqual(Guid.Empty, entity.HabitTypeId); // Deve ser gerado um novo GUID
        }

        [TestMethod]
        public void ToDto_ShouldMapAllPropertiesFromEntity()
        {
            // Arrange
            var entity = new HabitType
            {
                HabitTypeId = Guid.NewGuid(),
                Name = "Água",
                Unit = "litros",
                Factor = 0.03M
            };

            // Act
            var dto = _mapper.ToDto(entity);

            // Assert
            Assert.AreEqual(entity.Name, dto.Name);
            Assert.AreEqual(entity.Unit, dto.Unit);
            Assert.AreEqual(entity.Factor, dto.Factor);
        }
    }
}
