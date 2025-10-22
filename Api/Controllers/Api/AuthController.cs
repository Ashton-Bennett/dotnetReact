using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Api
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {

            var user = await _userService.ValidateUserAsync(model.Email, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // store refresh token securely (e.g., in DB)
            await _userService.SaveRefreshTokenAsync(user.Id, refreshToken);

            // send refresh token as HttpOnly cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new LoginResponse
            {
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles,
                AccessToken = accessToken
            });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
                return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _userService.ReplaceRefreshTokenAsync(user.Id, refreshToken, newRefreshToken);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { AccessToken = newAccessToken });
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
