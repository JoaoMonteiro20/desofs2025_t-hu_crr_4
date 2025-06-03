using EcoImpact.API.Mapper;
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
        private Mock<IUserValidator> _userValidatorMock = null!;
        private Mock<IUserMapper> _userMapperMock = null!;
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
            _userValidatorMock = new Mock<IUserValidator>();
            _userMapperMock = new Mock<IUserMapper>();

            _userService = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions,
                _userValidatorMock.Object,
                _userMapperMock.Object);
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldHashPassword_AndSaveUser()
        {
            var dto = new CreateUserDto
            {
                UserName = "testuser",
                Password = "plainPassword",
                Email = "testuser@example.com",
                Role = UserRole.User
            };

            _passwordServiceMock
                .Setup(p => p.HashPassword(It.IsAny<User>(), dto.Password))
                .Returns("hashedPassword");

            var result = await _userService.CreateAsync(dto);

            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual("testuser", result.UserName);
            Assert.AreEqual("hashedPassword", result.Password);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateFields_WhenUserExists()
        {
            Guid userId = Guid.NewGuid();
            var user = new User { UserId = Guid.NewGuid(), UserName = "old", Email = "old@email.com", Password = "oldpwd" };

            _mockSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(user);
            _userMapperMock.Setup(m => m.UpdateUserFromDto(user, It.IsAny<UserUpdateDto>()));

            var updateDto = new UserUpdateDto { UserName = "newName" };

            var result = await _userService.UpdateAsync(userId, updateDto);

            Assert.IsNotNull(result);
            _userValidatorMock.Verify(v => v.Validate(updateDto), Times.Once);
            _userMapperMock.Verify(m => m.UpdateUserFromDto(user, updateDto), Times.Once);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            _mockSet.Setup(m => m.FindAsync(userId)).ReturnsAsync((User?)null);

            var dto = new UserUpdateDto { Email = "new@email.com" };
            var result = await _userService.UpdateAsync(userId, dto);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateOnlyProvidedFields()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                UserName = "oldname",
                Email = "old@email.com",
                Password = "oldpass"
            };

            var dto = new UserUpdateDto
            {
                UserName = "newname"
            };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(existingUser);

            var validatorMock = new Mock<IUserValidator>();
            var mapperMock = new Mock<IUserMapper>();
            mapperMock.Setup(m => m.UpdateUserFromDto(existingUser, dto)).Callback(() => existingUser.UserName = dto.UserName);

            var service = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions,
                validatorMock.Object,
                mapperMock.Object);

            var updated = await service.UpdateAsync(existingUser.UserId, dto);

            Assert.IsNotNull(updated);
            Assert.AreEqual("newname", updated!.UserName);
            Assert.AreEqual("old@email.com", updated.Email); // unchanged
        }

        [TestMethod]
        public async Task CreateAsync_ShouldThrow_WhenInvalidDto()
        {
            var dto = new CreateUserDto
            {
                UserName = "",
                Email = "invalidemail",
                Password = "short"
            };

            var validatorMock = new Mock<IUserValidator>();
            validatorMock.Setup(v => v.Validate(dto)).Throws(new ArgumentException("Invalid data"));

            var service = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions,
                validatorMock.Object,
                Mock.Of<IUserMapper>());

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => service.CreateAsync(dto));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnNull_WhenUserNotFound()
        {
            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User?)null);

            var service = new UserService(
                _mockContext.Object,
                _passwordServiceMock.Object,
                NullLogger<UserService>.Instance,
                _defaultJsonOptions,
                Mock.Of<IUserValidator>(),
                Mock.Of<IUserMapper>());

            var result = await service.UpdateAsync(Guid.NewGuid(), new UserUpdateDto());

            Assert.IsNull(result);
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
