using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User> CreateAsync(User newUser)
        {
            var result = await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(User userToDelete)
        {
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<User?> GetByUsernameAndPasswordAsync(string email, string password) =>
            await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            var users = await GetAllAsync();
            return users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }
    }
}
