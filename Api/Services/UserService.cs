using Api.Enums;
using Api.Models.Data;
using Api.Models.DTOs;
using Api.Repositories;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository repo, IPasswordService passwordService)
        {
            _repo = repo;
            _passwordService = passwordService;
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetAllAsync()
        {
            try
            {
                var listOfUsers = await _repo.GetAllAsync();
                return ServiceResult<IEnumerable<User>>.Ok(listOfUsers);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.Fail("An unexpected error occurred.");
            }

        }

        public async Task<ServiceResult<User>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _repo.GetByIdAsync(id);
                return ServiceResult<User>.Ok(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Fail("An unexpected error occurred.");
            }

        }

        public async Task<ServiceResult<IEnumerable<RoleDto>>> GetRolesAsync()
        {
            try
            {
                var roles = EnumExtensions.ToRoleDtoList();
                return ServiceResult<IEnumerable<RoleDto>>.Ok(roles);
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while retrieving roles.";
                return ServiceResult<IEnumerable<RoleDto>>.Fail(errorMessage);

            }

        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _repo.GetByEmailAsync(email);

        public async Task<ServiceResult<User>> CreateAsync(User newUser)
        {
            if (await _repo.ExistsByEmailAsync(newUser.Email))
                return ServiceResult<User>.Fail("Email already registered");

            var passwordError = _passwordService.ValidatePasswordComplexity(newUser.Password);
            if (passwordError != null)
                return ServiceResult<User>.Fail(passwordError);

            newUser.Password = _passwordService.HashPassword(newUser.Password);

            var createdUser = await _repo.CreateAsync(newUser);
            return ServiceResult<User>.Ok(createdUser);
        }

        public async Task<ServiceResult<bool>> Delete(int id)
        {
            try
            {
                var userToDelete = await _repo.GetByIdAsync(id);
                if (userToDelete == null)
                {
                    return ServiceResult<bool>.Ok(false);
                }
                await _repo.DeleteAsync(userToDelete);
                await _repo.SaveChangesAsync();
                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Unable to delete user.");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, User user)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                {
                    return ServiceResult<bool>.Fail("User id not found.");
                }

                existing.Email = user.Email;
                existing.Username = user.Username;
                existing.Roles = user.Roles;
                existing.DateOfBirth = user.DateOfBirth;
                await _repo.SaveChangesAsync();
                return ServiceResult<bool>.Ok(true);
            } 
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Unable to delete user.");
            }
        }

        public async Task<ServiceResult<object>> UpdatePasswordAsync(int id, string currentPasswordFromClient, string newPasswordFromClient)
        {
            try
            {
                var passwordError = _passwordService.ValidatePasswordComplexity(newPasswordFromClient);
                if (passwordError != null)
                    return ServiceResult<object>.Fail(passwordError);

                var existing = await _repo.GetByIdAsync(id);
                if (existing == null) return ServiceResult<object>.Fail("Unable to update user please refresh the page and try again.");

                if (!_passwordService.VerifyPassword(currentPasswordFromClient, existing.Password))
                {
                    return ServiceResult<object>.Fail("Current password doesn't match our records.");
                }

                existing.Password = _passwordService.HashPassword(newPasswordFromClient);

                await _repo.SaveChangesAsync();
                return ServiceResult<object>.Ok("Saved new password, you will now be redirected to login.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Fail("An unexpected error occurred.");
            }

        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _repo.GetByRefreshTokenAsync(refreshToken);
        }

        public async Task ReplaceRefreshTokenAsync(int userId, string oldRefreshToken, string newRefreshToken)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user != null && user.RefreshToken == oldRefreshToken)
            {
                user.RefreshToken = newRefreshToken;
                await _repo.SaveChangesAsync();
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _repo.GetByRefreshTokenAsync(refreshToken);
            if (user != null)
            {
                user.RefreshToken = null;
                await _repo.SaveChangesAsync();
            }
        }
    }
}