using Api.Models;

namespace Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<User> CreateAsync(User newUser);

        Task<bool> Delete(int id);

        Task<bool> UpdateAsync(int id, User newUser);

        Task<User?> ValidateUserAsync(string email, string password);

        Task SaveRefreshTokenAsync(int userId, string refreshToken);

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task ReplaceRefreshTokenAsync(int userId, string oldRefreshToken, string newRefreshToken);

        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
