using Api.Models.Data;

namespace Api.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
        string? ValidatePasswordComplexity(string password);
    }

}
