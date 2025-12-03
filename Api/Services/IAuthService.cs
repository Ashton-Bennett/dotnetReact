using Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public interface IAuthService
    {
        public Task<ServiceResult<LoginResponse>> LoginAsync(string email, string password);

        public Task<ServiceResult<AuthTokensDTO>> RefreshAsync(string? currentToken);

        public Task<IActionResult> LogoutAsync();

        public Task<ServiceResult<LoginResponse>> Validate(string? currentRefreshToken);
    }
}
