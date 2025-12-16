using Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(IUserService userService, ITokenService tokenService, IPasswordService passwordService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<ServiceResult<LoginResponse>> LoginAsync(string email, string password)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<LoginResponse>.Fail("Invalid email");
            }

            bool validPassword = _passwordService.VerifyPassword(password, user.Password);
            if (!validPassword)
            {
                return ServiceResult<LoginResponse>.Fail("Invalid password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            return ServiceResult<LoginResponse>.Ok(new LoginResponse
            {
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles,
                AccessToken = accessToken,
                Id = user.Id
            });

        }

        public async Task<ServiceResult<AuthTokensDTO>> RefreshAsync(string? currentRefreshToken)
        {

            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                return ServiceResult<AuthTokensDTO>.Fail("Unauthorized");
            }

            var user = await _userService.GetUserByRefreshTokenAsync(currentRefreshToken);
            if (user == null)
            {
                return ServiceResult<AuthTokensDTO>.Fail("Unauthorized");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _userService.ReplaceRefreshTokenAsync(user.Id, currentRefreshToken, newRefreshToken);

            var returnValue = new AuthTokensDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return ServiceResult<AuthTokensDTO>.Ok(returnValue);
        }

        public async Task<ServiceResult<LoginResponse>> Validate(string? currentRefreshToken)
        {

            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                return ServiceResult<LoginResponse>.Fail("Unauthorized");
            }

            var user = await _userService.GetUserByRefreshTokenAsync(currentRefreshToken);
            if (user == null)
            {
                return ServiceResult<LoginResponse>.Fail("Unauthorized");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);

            return ServiceResult<LoginResponse>.Ok(new LoginResponse
            {
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles,
                AccessToken = newAccessToken,
                Id = user.Id
            });
        }

        public Task<IActionResult> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
