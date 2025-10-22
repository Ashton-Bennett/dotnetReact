using Api.Models;
using Api.Repositories;

namespace Api.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo) => _repo = repo;

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<User> CreateAsync(User newUser) =>
            await _repo.CreateAsync(newUser);

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

            existing.Username = user.Username;
            existing.Roles = user.Roles; //was changed to an array will need to update or test
            existing.Password = user.Password;
            existing.DateOfBirth = user.DateOfBirth;

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<User?> ValidateUserAsync(string email, string password) =>
            await _repo.GetByUsernameAndPasswordAsync(email, password);

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