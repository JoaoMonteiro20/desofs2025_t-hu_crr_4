using EcoImpact.API.Services;
using EcoImpact.DataModel;
using EcoImpact.DataModel.Dtos;
using EcoImpact.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EcoImpact.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<DbSet<User>> _mockSet = null!;
        private Mock<EcoDbContext> _mockContext = null!;
        private Mock<IPasswordService> _passwordServiceMock = null!;
        private UserService _userService = null!;

        private readonly JsonSerializerOptions _defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
            return mockSet;
        }

        [TestInitialize]
        public void Setup()
        {
            var users = new List<User>().AsQueryable();
            _mockSet = CreateMockDbSet(users);

            _mockContext = new Mock<EcoDbContext>();
            _mockContext.Setup(c => c.Users).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _passwordServiceMock = new Mock<IPasswordService>();

            _userService = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions);
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldHashPassword_AndSaveUser()
        {
            var user = new User
            {
                UserName = "testuser",
                Password = "plainPassword",
                Email = "testuser@example.com"
            };

            _passwordServiceMock
                .Setup(p => p.HashPassword(It.IsAny<User>(), "plainPassword"))
                .Returns("hashedPassword");

            var result = await _userService.CreateAsync(user);

            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual("testuser", result.UserName);
            Assert.AreEqual("hashedPassword", result.Password);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "existinguser",
                Password = "pwd",
                Email = "asda"
            };

            var users = new List<User> { user };
            _mockSet = CreateMockDbSet(users);

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync((object[] ids) =>
                    {
                        var id = (Guid)ids[0];
                        return users.FirstOrDefault(u => u.UserId == id);
                    });

            _mockContext.Setup(c => c.Users).Returns(_mockSet.Object);

            _userService = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions);

            var result = await _userService.GetByIdAsync(user.UserId);

            Assert.IsNotNull(result);
            Assert.AreEqual("existinguser", result!.UserName);
        }

        [TestMethod]
        public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var users = new List<User>(); // nenhum user
            _mockSet = CreateMockDbSet(users);
            _mockContext.Setup(c => c.Users).Returns(_mockSet.Object);

            _userService = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions);

            var result = await _userService.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            var user = new User { UserId = Guid.NewGuid(), UserName = "todelete", Password = "pwd", Email = "adas" };
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);

            var result = await _userService.DeleteAsync(user.UserId);

            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User?)null);

            var result = await _userService.DeleteAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ExportUsersAsJsonFileAsync_ShouldReturnValidJsonFile()
        {
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new EcoDbContext(options);

            context.Users.AddRange(new List<User>
            {
                new User { UserId = Guid.NewGuid(), UserName = "admin", Email = "admin@example.com", Role = UserRole.Admin, Password = "xxx" },
                new User { UserId = Guid.NewGuid(), UserName = "user1", Email = "user1@example.com", Role = UserRole.User, Password = "yyy" }
            });

            await context.SaveChangesAsync();

            var service = new UserService(
                context,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions);

            var result = await service.ExportUsersAsJsonFileAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("application/json", result.ContentType);
            Assert.AreEqual("users_export.json", result.FileName);
            Assert.IsTrue(result.FileContent.Length > 0);

            var json = JsonSerializer.Deserialize<List<UserExportDto>>(result.FileContent, _defaultJsonOptions);
            Assert.IsNotNull(json);
            Assert.AreEqual(2, json!.Count);
            Assert.IsTrue(json.Any(u => u.UserName == "admin"));
            Assert.IsTrue(json.Any(u => u.UserName == "user1"));
        }
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        public T Current => _inner.Current;
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    }
}
