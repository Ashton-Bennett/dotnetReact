using Api.Models.Data;

namespace Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string id);
        Task<User> CreateAsync(User newUser);
        Task DeleteAsync(User userToDelete);
        Task SaveChangesAsync();
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
