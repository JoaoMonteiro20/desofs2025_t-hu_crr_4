using EcoImpact.DataModel;
using EcoImpact.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EcoImpact.Tests.Services
{
    [TestClass]
    public class HabitTypeServiceTests
    {
        private Mock<DbSet<HabitType>> _mockSet = null!;
        private Mock<EcoDbContext> _mockContext = null!;
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

        [TestInitialize]
        public void Setup()
        {
            var data = new List<HabitType>().AsQueryable();
            _mockSet = CreateMockDbSet(data);

            _mockContext = new Mock<EcoDbContext>();
            _mockContext.Setup(c => c.HabitTypes).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _service = new HabitTypeService(_mockContext.Object, NullLogger<HabitTypeService>.Instance);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldAddHabitType()
        {
            var dto = new HabitTypeDto { Name = "Transporte", Unit = "km", Factor = 0.21M };

            var result = await _service.CreateAsync(dto);

            _mockSet.Verify(m => m.Add(It.IsAny<HabitType>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.Unit, result.Unit);
            Assert.AreEqual(dto.Factor, result.Factor);
        }


        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnHabitType_WhenExists()
        {
            var habit = new HabitType { HabitTypeId = Guid.NewGuid(), Name = "Comida", Unit = "kg", Factor = 0.8M };
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync((object[] ids) =>
                    {
                        var id = (Guid)ids[0];
                        return id == habit.HabitTypeId ? habit : null;
                    });

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
    }
}
