using Api.Models.Data;
using Api.Models.DTOs;

namespace Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);

        Task<ServiceResult<User>> CreateAsync(User newUser);

        Task<bool> Delete(int id);

        Task<bool> UpdateAsync(int id, User newUser);

        Task<ServiceResult<object>> UpdatePasswordAsync(int id, string currentPasswordFromClient, string newPasswordFromClient);

        Task SaveRefreshTokenAsync(int userId, string refreshToken);

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task ReplaceRefreshTokenAsync(int userId, string oldRefreshToken, string newRefreshToken);

        Task RevokeRefreshTokenAsync(string refreshToken);

        bool VerifyPassword(User user, string enteredPassword, string passwordHash);
    }
}
