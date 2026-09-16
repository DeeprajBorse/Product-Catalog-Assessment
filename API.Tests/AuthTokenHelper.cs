using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Tests
{
    public static class AuthTokenHelper
    {
        private const string SecretKey = "KYePDI1a5We5ddfgtKIHWaPMx9W6s3EcW1knjfWk2o1";
        private const string Issuer = "Issuer";
        private const string Audience = "Audience";

        public static string GenerateToken(string role = "Admin", string email = "test@example.com", string name = "TestUser")
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}