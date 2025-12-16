using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tests.UnitTests.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_WhenCalled_ReturnsSaltAndHashSeparatedByDot()
        {
            // Act
            var result = _passwordService.HashPassword("Password123");

            // Assert
            Assert.NotNull(result);

            var parts = result.Split('.');
            Assert.Equal(2, parts.Length);
            Assert.False(string.IsNullOrWhiteSpace(parts[0]));
            Assert.False(string.IsNullOrWhiteSpace(parts[1]));
        }

        [Fact]
        public void HashPassword_WhenCalledMultipleTimes_ProducesDifferentHashes()
        {
            // Act
            var hash1 = _passwordService.HashPassword("Password123");
            var hash2 = _passwordService.HashPassword("Password123");

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WhenPasswordMatches_ReturnsTrue()
        {
            // Arrange
            var password = "Password123";
            var hash = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WhenPasswordDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var hash = _passwordService.HashPassword("Password123");

            // Act
            var result = _passwordService.VerifyPassword("WrongPassword", hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_ReturnsUrlSafeBase64()
        {
            // Act
            var hash = _passwordService.HashPassword("Password123");

            // Assert
            Assert.DoesNotContain("+", hash);
            Assert.DoesNotContain("/", hash);
            Assert.DoesNotContain("=", hash);
        }

        [Fact]
        public void ValidatePasswordComplexity_WhenTooShort_ReturnsError()
        {
            var result = _passwordService.ValidatePasswordComplexity("Ab1");

            Assert.Equal("Password must be at least 8 characters.", result);
        }

        [Fact]
        public void ValidatePasswordComplexity_WhenMissingNumber_ReturnsError()
        {
            var result = _passwordService.ValidatePasswordComplexity("Password");

            Assert.Equal("Password must contain a number.", result);
        }

        [Fact]
        public void ValidatePasswordComplexity_WhenMissingUppercase_ReturnsError()
        {
            var result = _passwordService.ValidatePasswordComplexity("password1");

            Assert.Equal("Password must contain an uppercase letter.", result);
        }

        [Fact]
        public void ValidatePasswordComplexity_WhenValid_ReturnsNull()
        {
            var result = _passwordService.ValidatePasswordComplexity("Password123");

            Assert.Null(result);
        }
    }
}
