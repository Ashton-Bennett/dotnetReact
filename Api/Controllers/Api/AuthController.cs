using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Api
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, ITokenService tokenService, IAuthService authService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _authService.LoginAsync(model.Email, model.Password);

            if (result == null)
                return BadRequest("Failed to login");

            if (!result.Success || result.Data == null)
                return BadRequest(new { message = result.ErrorMessage });

            var refreshToken = _tokenService.GenerateRefreshToken();

            // store refresh token securely (e.g., in DB)
            await _userService.SaveRefreshTokenAsync(result.Data.Id, refreshToken);

            // Send refresh token as HttpOnly cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            var userResponse = new LoginResponse
            {
                Email = result.Data.Email,
                Username = result.Data.Username,
                Roles = result.Data.Roles,
                AccessToken = result.Data.AccessToken,
                Id = result.Data.Id,
            };

            return Ok(userResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Missing refresh token." });
            }

            var result = await _authService.RefreshAsync(refreshToken);

            if (result == null)
                return Unauthorized(new { message = "Failed to refresh token." });

            if (!result.Success || result.Data == null)
                return Unauthorized(new { message = result.ErrorMessage ?? "Invalid refresh token." });
            

            Response.Cookies.Append("refreshToken", result.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { result.Data.AccessToken });
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Missing refresh token." });
            }

            var result = await _authService.Validate(refreshToken);

            if (result == null)
                return Unauthorized(new { message = "Failed to refresh token." });

            if (!result.Success || result.Data == null)
                return Unauthorized(new { message = result.ErrorMessage ?? "Invalid refresh token." });

            var userResponse = new LoginResponse
            {
                Email = result.Data.Email,
                Username = result.Data.Username,
                Roles = result.Data.Roles,
                AccessToken = result.Data.AccessToken,
                Id = result.Data.Id,
            };

            return Ok(userResponse);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _userService.RevokeRefreshTokenAsync(refreshToken);
                Response.Cookies.Delete("refreshToken");
            }

            return Ok(new { message = "Logged out successfully." });
        }
    }
}
