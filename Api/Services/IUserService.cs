using Api.Models.Data;
using Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<User>>> GetAllAsync();

        Task<ServiceResult<User>> GetByIdAsync(int id);

        Task<User?> GetByEmailAsync(string email);

        Task<ServiceResult<IEnumerable<RoleDto>>> GetRolesAsync();
            
        Task<ServiceResult<User>> CreateAsync(User newUser);

        Task<ServiceResult<bool>> Delete(int id);

        Task<ServiceResult<bool>> UpdateAsync(int id, User newUser);

        Task<ServiceResult<object>> UpdatePasswordAsync(int id, string currentPasswordFromClient, string newPasswordFromClient);

        Task SaveRefreshTokenAsync(int userId, string refreshToken);

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task ReplaceRefreshTokenAsync(int userId, string oldRefreshToken, string newRefreshToken);

        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
