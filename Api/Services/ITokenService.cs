using Api.Models;

namespace Api.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
