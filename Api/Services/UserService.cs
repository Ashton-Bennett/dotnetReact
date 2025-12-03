using Api.Models.Data;
using Api.Models.DTOs;
using Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Api.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _repo;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserService(IUserRepository repo) => _repo = repo;

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _repo.GetByEmailAsync(email);


        public async Task<ServiceResult<User>> CreateAsync(User newUser)
        {
            if (await _repo.ExistsByEmailAsync(newUser.Email))
                return ServiceResult<User>.Fail("Email already registered");

            var passwordError = ValidatePasswordComplexity(newUser.Password);
            if (passwordError != null)
                return ServiceResult<User>.Fail(passwordError);

            newUser.Password = HashPassword(newUser, newUser.Password);

            var createdUser = await _repo.CreateAsync(newUser);
            return ServiceResult<User>.Ok(createdUser);
        }

        static string? ValidatePasswordComplexity(string password)
        {
            if (password.Length < 8)
                return "Password must be at least 8 characters long.";
            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter.";
            if (!password.Any(char.IsDigit))
                return "Password must contain at least one number.";
            if (!password.Any(ch => "!@#$%^&*()".Contains(ch)))
                return "Password must contain at least one special character.";

            return null; // password is valid
        }

        public async Task<bool> Delete(int id)
        {
            var userToDelete = await _repo.GetByIdAsync(id);
            if (userToDelete == null) return false;

            await _repo.DeleteAsync(userToDelete);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Email = user.Email;
            existing.Username = user.Username;
            existing.Roles = user.Roles;
            existing.DateOfBirth = user.DateOfBirth;

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResult<object>> UpdatePasswordAsync(int id, string currentPasswordFromClient, string newPasswordFromClient)
        {

            var passwordError = ValidatePasswordComplexity(newPasswordFromClient);
            if (passwordError != null)
                return ServiceResult<object>.Fail(passwordError);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return ServiceResult<object>.Fail("Unable to update user please refresh the page and try again.");

            if (!VerifyPassword(existing, currentPasswordFromClient, existing.Password))
            {
                return ServiceResult<object>.Fail("Current password doesn't match our records.");
            }

            existing.Password = HashPassword(existing, newPasswordFromClient);

            await _repo.SaveChangesAsync();
            return ServiceResult<object>.Ok("Saved new password, you will now be redirected to login."); ;
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

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}