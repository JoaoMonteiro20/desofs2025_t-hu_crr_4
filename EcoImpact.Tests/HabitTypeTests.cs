using EcoImpact.DataModel;
using EcoImpact.DataModel.Models;
using EcoImpact.API.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using EcoImpact.API.Services;

namespace EcoImpact.Tests
{
    [TestClass]
    public class HabitTypeServiceTests
    {
        private Mock<DbSet<HabitType>> _mockSet = null!;
        private Mock<EcoDbContext> _mockContext = null!;
        private Mock<IHabitTypeMapper> _mockMapper = null!;
        private HabitTypeService _service = null!;

        private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static EcoDbContext CreateEmptyInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // garante base isolada por teste
                .Options;

            return new EcoDbContext(options);
        }

        [TestInitialize]
        public void Setup()
        {
            var data = new List<HabitType>().AsQueryable();
            _mockSet = CreateMockDbSet(data);

            _mockContext = new Mock<EcoDbContext>();
            _mockContext.Setup(c => c.HabitTypes).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockMapper = new Mock<IHabitTypeMapper>();
            _service = new HabitTypeService(_mockContext.Object, NullLogger<HabitTypeService>.Instance, _mockMapper.Object);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldAddHabitType()
        {
            var dto = new HabitTypeDto { Name = "Transporte", Unit = "km", Factor = 0.21M };
            var habitEntity = new HabitType { HabitTypeId = Guid.NewGuid(), Name = dto.Name, Unit = dto.Unit, Factor = dto.Factor };

            _mockMapper.Setup(m => m.ToEntity(dto)).Returns(habitEntity);

            var result = await _service.CreateAsync(dto);

            _mockSet.Verify(m => m.Add(It.Is<HabitType>(h => h.Name == dto.Name)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.Unit, result.Unit);
            Assert.AreEqual(dto.Factor, result.Factor);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnHabitType_WhenExists()
        {
            var habit = new HabitType { HabitTypeId = Guid.NewGuid(), Name = "Comida", Unit = "kg", Factor = 0.8M };
            var dto = new HabitTypeDto { Name = habit.Name, Unit = habit.Unit, Factor = habit.Factor };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync((object[] ids) =>
                    {
                        var id = (Guid)ids[0];
                        return id == habit.HabitTypeId ? habit : null;
                    });

            _mockMapper.Setup(m => m.ToDto(habit)).Returns(dto);

            var result = await _service.GetByIdAsync(habit.HabitTypeId);

            Assert.IsNotNull(result);
            Assert.AreEqual(habit.Name, result!.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((HabitType?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateFields_WhenExists()
        {
            var habit = new HabitType { HabitTypeId = Guid.NewGuid(), Name = "Comida", Unit = "kg", Factor = 0.8M };
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(habit);

            var dto = new HabitTypeDto { Name = "Alimentação", Unit = "g", Factor = 1.2M };

            var updatedDto = new HabitTypeDto
            {
                Name = dto.Name,
                Unit = dto.Unit,
                Factor = dto.Factor
            };

            _mockMapper.Setup(m => m.ToDto(habit)).Returns(updatedDto);

            var result = await _service.UpdateAsync(habit.HabitTypeId, dto);

            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result!.Name);
            Assert.AreEqual(dto.Unit, result.Unit);
            Assert.AreEqual(dto.Factor, result.Factor);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
        {
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((HabitType?)null);

            var dto = new HabitTypeDto { Name = "Nova", Unit = "x", Factor = 0.1M };

            var result = await _service.UpdateAsync(Guid.NewGuid(), dto);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDelete_WhenExists()
        {
            var habit = new HabitType { HabitTypeId = Guid.NewGuid(), Name = "Água", Unit = "litros", Factor = 0.02M };
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(habit);

            var result = await _service.DeleteAsync(habit.HabitTypeId);

            _mockSet.Verify(m => m.Remove(It.IsAny<HabitType>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
        {
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((HabitType?)null);

            var result = await _service.DeleteAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ImportFromFileAsync_ShouldImportHabitTypes()
        {
            var dtos = new List<HabitTypeDto>
        {
        new HabitTypeDto { Name = "Dieta", Unit = "kg", Factor = 0.6M },
        new HabitTypeDto { Name = "Luz", Unit = "kWh", Factor = 0.3M }
        };

            var jsonContent = JsonSerializer.Serialize(dtos);
            var byteArray = Encoding.UTF8.GetBytes(jsonContent);
            var stream = new MemoryStream(byteArray);
            var formFile = new FormFile(stream, 0, byteArray.Length, "file", "habits.json")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/json"
            };

            foreach (var dto in dtos)
            {
                _mockMapper.Setup(m => m.ToEntity(dto)).Returns(new HabitType
                {
                    HabitTypeId = Guid.NewGuid(),
                    Name = dto.Name,
                    Unit = dto.Unit,
                    Factor = dto.Factor
                });
            }

            _mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<HabitType>>()));
            _mockContext.Setup(c => c.HabitTypes).Returns(_mockSet.Object);

            var result = await _service.ImportFromFileAsync(formFile);

            Assert.IsTrue(result.Contains("2 habit types imported successfully."));
            _mockSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<HabitType>>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        
    }
}
