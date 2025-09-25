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

        public async Task<User> Create(User newUser) =>
            await _repo.CreateAsync(newUser);
    }
}
