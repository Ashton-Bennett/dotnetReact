using Api.Models.Data;
using Api.Services;
using Moq;

namespace Api.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService(
                _userServiceMock.Object,
                _tokenServiceMock.Object,
                _passwordServiceMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsLoginResponse()
        {
            // Arrange
            var email = "test@test.com";
            var password = "password123";

            var user = new User
            {
                Id = 1,
                Email = email,
                Username = "testuser",
                Password = "hashed-password",
                Roles = new List<string> { "Admin" }
            };

            _userServiceMock
                .Setup(s => s.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(s => s.VerifyPassword(password, user.Password))
                .Returns(true);

            _tokenServiceMock
                .Setup(s => s.GenerateAccessToken(user))
                .Returns("access-token");

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(email, result.Data!.Email);
            Assert.Equal("access-token", result.Data.AccessToken);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _userServiceMock
                .Setup(s => s.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync("bad@test.com", "password");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email", result.ErrorMessage);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsInvalid_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Password = "hashed"
            };

            _userServiceMock
                .Setup(s => s.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(s => s.VerifyPassword(It.IsAny<string>(), user.Password))
                .Returns(false);

            // Act
            var result = await _authService.LoginAsync(user.Email, "wrong-password");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid password", result.ErrorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RefreshAsync_WhenTokenIsNullOrEmpty_ReturnsUnauthorized(string? token)
        {
            // Act
            var result = await _authService.RefreshAsync(token);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unauthorized", result.ErrorMessage);
        }

        [Fact]
        public async Task RefreshAsync_WhenUserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _userServiceMock
                .Setup(s => s.GetUserByRefreshTokenAsync("bad-token"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.RefreshAsync("bad-token");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unauthorized", result.ErrorMessage);
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenIsValid_ReturnsNewTokens()
        {
            // Arrange
            var user = new User { Id = 1, Email="test@test.com" };

            _userServiceMock
                .Setup(s => s.GetUserByRefreshTokenAsync("valid-token"))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(s => s.GenerateAccessToken(user))
                .Returns("new-access-token");

            _tokenServiceMock
                .Setup(s => s.GenerateRefreshToken())
                .Returns("new-refresh-token");

            // Act
            var result = await _authService.RefreshAsync("valid-token");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("new-access-token", result.Data!.AccessToken);
            Assert.Equal("new-refresh-token", result.Data.RefreshToken);

            _userServiceMock.Verify(
                s => s.ReplaceRefreshTokenAsync(user.Id, "valid-token", "new-refresh-token"),
                Times.Once
            );
        }

        [Fact]
        public async Task Validate_WhenTokenIsMissing_ReturnsUnauthorized()
        {
            var result = await _authService.Validate(null);

            Assert.False(result.Success);
            Assert.Equal("Unauthorized", result.ErrorMessage);
        }

        [Fact]
        public async Task Validate_WhenTokenIsValid_ReturnsLoginResponse()
        {
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Username = "test",
                Roles = new List<string> { "User" }
            };

            _userServiceMock
                .Setup(s => s.GetUserByRefreshTokenAsync("valid-token"))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(s => s.GenerateAccessToken(user))
                .Returns("access-token");

            var result = await _authService.Validate("valid-token");

            Assert.True(result.Success);
            Assert.Equal("access-token", result.Data!.AccessToken);
        }


    }
}
