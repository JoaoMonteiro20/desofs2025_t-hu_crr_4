using EcoImpact.API.Services;
using EcoImpact.DataModel;
using EcoImpact.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EcoImpact.Tests
{
    [TestClass]
    public class AuthenticationServiceTests
    {
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

        private static EcoDbContext CreateEmptyInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new EcoDbContext(options);
        }

        private static IConfiguration MockJwtConfig()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Jwt:Key"]).Returns("very_secure_jwt_key_for_tests_12345"); // ≥ 32 chars
            return config.Object;
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldBlockUserAfterFourFailedAttempts()
        {
            // Arrange: usar EF real com base de dados in-memory
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new EcoDbContext(options);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "secureuser",
                Email = "secure@example.com",
                Password = "hashedPwd",
                FailedLoginAttempts = 0,
                LockoutEnd = null,
                Role = UserRole.User
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var passwordService = new Mock<IPasswordService>();
            passwordService.Setup(p => p.VerifyPassword(user, user.Password, It.IsAny<string>()))
                           .Returns(false); // password errada

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Jwt:Key"]).Returns("secure_test_key_123");

            var authService = new AuthenticationService(
                context,
                passwordService.Object,
                config.Object,
                NullLogger<AuthenticationService>.Instance
            );

            // Act: 4 tentativas falhadas
            for (int i = 0; i < 4; i++)
            {
                var token = await authService.AuthenticateAsync("secureuser", "wrongpassword");
                Assert.IsNull(token);
            }

            // Assert: bloqueado
            var updatedUser = await context.Users.FirstAsync();
            Assert.AreEqual(4, updatedUser.FailedLoginAttempts);
            Assert.IsNotNull(updatedUser.LockoutEnd);
            Assert.IsTrue(updatedUser.LockoutEnd > DateTime.UtcNow);

            // 5ª tentativa → exceção
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                authService.AuthenticateAsync("secureuser", "wrongpassword")
            );
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldResetFailedAttemptsAfterSuccess()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new EcoDbContext(options);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "resetuser",
                Email = "reset@example.com",
                Password = "hashed",
                FailedLoginAttempts = 3,
                LockoutEnd = null,
                Role = UserRole.User
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var passwordService = new Mock<IPasswordService>();
            passwordService.Setup(p => p.VerifyPassword(user, user.Password, It.IsAny<string>()))
                           .Returns(true); // password correta

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Jwt:Key"]).Returns("super_secure_test_key_that_is_32_bytes!");

            var authService = new AuthenticationService(
                context,
                passwordService.Object,
                config.Object,
                NullLogger<AuthenticationService>.Instance
            );

            // Act
            var token = await authService.AuthenticateAsync("resetuser", "correctpassword");

            // Assert
            Assert.IsNotNull(token);
            var updatedUser = await context.Users.FirstAsync();
            Assert.AreEqual(0, updatedUser.FailedLoginAttempts);
            Assert.IsNull(updatedUser.LockoutEnd);
        }

        [TestMethod]
        public async Task Authenticate_Should_ReturnNull_IfUserDoesNotExist()
        {
            var context = CreateEmptyInMemoryContext();
            var passwordService = new Mock<IPasswordService>();
            var config = MockJwtConfig();

            var service = new AuthenticationService(context, passwordService.Object, config, NullLogger<AuthenticationService>.Instance);

            var result = await service.AuthenticateAsync("notfound", "any");

            Assert.IsNull(result); // resposta genérica
        }


    }
}
