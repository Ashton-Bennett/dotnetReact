using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Models
{
    public class TokenGeneratorForTesting
    {

        public static string Generate(WebApplicationBuilder builder, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "ashtonb"),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: builder.Configuration["Authentication:Schemes:LocalAuthIssuer:ValidIssuer"],
                audience: builder.Configuration.GetSection("Authentication:Schemes:LocalAuthIssuer:ValidAudiences").Get<string[]>()[0],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
