using EcoImpact.API.Services;
using EcoImpact.DataModel.Dtos;

namespace EcoImpact.Tests
{
    [TestClass]
    public class UserValidatorTests
    {
        private UserValidator _validator = null!;

        [TestInitialize]
        public void Setup()
        {
            _validator = new UserValidator();
        }

        [TestMethod]
        public void ValidateCreateUserDto_ShouldPass_WithValidData()
        {
            var dto = new CreateUserDto
            {
                UserName = "validUser",
                Email = "user@example.com",
                Password = "Valid123!"
            };

            _validator.Validate(dto); // Should not throw
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCreateUserDto_ShouldFail_WhenUsernameMissing()
        {
            var dto = new CreateUserDto
            {
                UserName = "",
                Email = "user@example.com",
                Password = "Valid123!"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCreateUserDto_ShouldFail_WhenEmailInvalid()
        {
            var dto = new CreateUserDto
            {
                UserName = "user",
                Email = "invalid-email",
                Password = "Valid123!"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCreateUserDto_ShouldFail_WhenPasswordTooShort()
        {
            var dto = new CreateUserDto
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "A1!"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCreateUserDto_ShouldFail_WhenPasswordMissingDigit()
        {
            var dto = new CreateUserDto
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "NoDigit!"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCreateUserDto_ShouldFail_WhenPasswordMissingSpecialChar()
        {
            var dto = new CreateUserDto
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "Valid123"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        public void ValidateUserUpdateDto_ShouldPass_WithPartialValidData()
        {
            var dto = new UserUpdateDto
            {
                Email = "update@example.com",
                Password = "NewValid1#"
            };

            _validator.Validate(dto); // Should not throw
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateUserUpdateDto_ShouldFail_WhenEmailInvalid()
        {
            var dto = new UserUpdateDto
            {
                Email = "invalid-email"
            };

            _validator.Validate(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateUserUpdateDto_ShouldFail_WhenPasswordInvalid()
        {
            var dto = new UserUpdateDto
            {
                Password = "weak"
            };

            _validator.Validate(dto);
        }
    }
}
