using Api.Models;

namespace Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User newUser);
        Task DeleteAsync(User userToDelete);
        Task SaveChangesAsync();
        Task<User?> GetByUsernameAndPasswordAsync(string email, string password);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
