using Api.Models.Data;
using Api.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public TokenServiceTests()
        {
            var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_TEST_KEY_1234567890123456" },
            { "Authentication:Schemes:LocalAuthIssuer:ValidIssuer", "TestIssuer" },
            { "Authentication:Schemes:LocalAuthIssuer:ValidAudiences:0", "TestAudience" }
        };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _tokenService = new TokenService(_configuration);
        }


        [Fact]
        public void GenerateAccessToken_WhenCalled_ReturnsValidJwt()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                Roles = new List<string> { "Admin" }
            };

            // Act
            var token = _tokenService.GenerateAccessToken(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));
        }

        [Fact]
        public void GenerateAccessToken_UsesConfiguredIssuerAndAudience()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "test",
                Email = "test@test.com",
                Roles = new List<string>()
            };

            // Act
            var token = _tokenService.GenerateAccessToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Contains("TestAudience", jwt.Audiences);
        }

        [Fact]
        public void GenerateAccessToken_IncludesExpectedClaims()
        {
            // Arrange
            var user = new User
            {
                Id = 42,
                Email = "test@test.com",
                Username = "ashton",
                Roles = new List<string> { "Admin", "User" }
            };

            // Act
            var token = _tokenService.GenerateAccessToken(user);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            Assert.Equal("ashton", jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal("42", jwt.Claims.First(c => c.Type == "UserId").Value);

            var roles = jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            Assert.Contains("Admin", roles);
            Assert.Contains("User", roles);
        }

        [Fact]
        public void GenerateAccessToken_HasExpiration()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "test",
                Email = "test@test.com",
                Roles = new List<string>()
            };

            // Act
            var token = _tokenService.GenerateAccessToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            Assert.NotNull(jwt.ValidTo);
            Assert.True(jwt.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateRefreshToken_WhenCalled_ReturnsNonEmptyString()
        {
            // Act
            var token = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateRefreshToken_WhenCalledMultipleTimes_ReturnsDifferentValues()
        {
            // Act
            var token1 = _tokenService.GenerateRefreshToken();
            var token2 = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsValidBase64()
        {
            // Act
            var token = _tokenService.GenerateRefreshToken();

            // Assert
            var bytes = Convert.FromBase64String(token);
            Assert.NotNull(bytes);
        }
    }
}
