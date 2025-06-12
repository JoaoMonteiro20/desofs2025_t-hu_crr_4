using EcoImpact.API.Services;
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

namespace EcoImpact.Tests
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private static EcoDbContext CreateEmptyInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EcoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new EcoDbContext(options);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldResetFailedAttemptsAfterSuccess()
        {
            // Arrange
            var context = CreateEmptyInMemoryContext();

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

            Environment.SetEnvironmentVariable("JWT", "super_secure_test_key_that_is_32_bytes!");

            var authService = new AuthenticationService(
                context,
                passwordService.Object,
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
        public async Task AuthenticateAsync_ShouldBlockUserAfterFourFailedAttempts()
        {
            var context = CreateEmptyInMemoryContext();

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
                           .Returns(false);

            Environment.SetEnvironmentVariable("JWT", "secure_test_key_123_secure_test_key_123");

            var authService = new AuthenticationService(
                context,
                passwordService.Object,
                NullLogger<AuthenticationService>.Instance
            );

            // Act
            for (int i = 0; i < 4; i++)
            {
                var token = await authService.AuthenticateAsync("secureuser", "wrongpassword");
                Assert.IsNull(token);
            }

            var updatedUser = await context.Users.FirstAsync();
            Assert.AreEqual(4, updatedUser.FailedLoginAttempts);
            Assert.IsNotNull(updatedUser.LockoutEnd);
            Assert.IsTrue(updatedUser.LockoutEnd > DateTime.UtcNow);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                authService.AuthenticateAsync("secureuser", "wrongpassword")
            );
        }

        [TestMethod]
        public async Task Authenticate_Should_ReturnNull_IfUserDoesNotExist()
        {
            var context = CreateEmptyInMemoryContext();
            var passwordService = new Mock<IPasswordService>();

            Environment.SetEnvironmentVariable("JWT", "very_secure_jwt_key_for_tests_1234567890!");

            var service = new AuthenticationService(context, passwordService.Object, NullLogger<AuthenticationService>.Instance);

            var result = await service.AuthenticateAsync("notfound", "any");

            Assert.IsNull(result);
        }
    }
}
