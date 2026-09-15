using API.Application.Interfaces;
using API.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Identity
{
    public class GenerateJWTToken(IConfiguration _config) : IGenerateJWTToken
    {
        public string GenerateAccessToken(Employee emp)
        {
            var secret = _config["JWT:SecretKey"];
            var issuer = _config["JWT:Issuer"];
            var audience = _config["JWT:Audience"];
            var expiryMinutes = _config.GetValue<int>("JWT:AccessTokenExpirationMinutes");

            if (string.IsNullOrEmpty(secret) || Encoding.UTF8.GetBytes(secret).Length < 16)
                throw new InvalidOperationException("JWT secret is missing or too short. Provide a secret at least 16 bytes long.");

            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT Issuer/Audience configuration is missing.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, emp.Name),
                new Claim(ClaimTypes.Email, emp.Email),
                new Claim(ClaimTypes.Role, emp.Role.ToString())
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return tokenhandler.WriteToken(token);
        }

        public (string Token, DateTime ExpiresOn) GenerateRefreshToken()
        {
            var refreshDays = _config.GetValue<int>("JWT:RefreshTokenExpirationDays");

            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);
            var expiresOn = DateTime.UtcNow.AddDays(refreshDays);

            return (token, expiresOn);
        }
    }
}
