using API.Application.DTO;
using API.Application.Interfaces;
using API.Domain.Entities;
using API.Domain.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unit;
        private readonly IGenerateJWTToken _token;
        private readonly IMapper _mapper;
        private readonly IAppLogger<AuthenticationService> _logger;

        public AuthenticationService( IUnitOfWork unit, IGenerateJWTToken token, IMapper mapper, IAppLogger<AuthenticationService> logger)
        {
            _unit = unit ;
            _token = token ;
            _mapper = mapper ;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> LoginAsync(AuthDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                _logger.LogError(new ArgumentNullException(nameof(dto)), "LoginAsync: AuthDTO is null.");
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                _logger.LogWarning("LoginAsync: Email is required.");
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Email is required." } }
                });
            }
                
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                _logger.LogError(new ArgumentNullException(nameof(dto)), "LoginAsync: Password is required.");
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "PasswordHash", new[] { "Password is required." } }
                });
            }
            
            // Find employee by email
            var existingEmp = await _unit.Employees.GetByAsync(
                e => e.Email == dto.Email,
                useNoTracking: true,
                cancellationToken: cancellationToken);

            if (existingEmp == null)
            {
                _logger.LogWarning("LoginAsync: Employee not found.");
                throw new NotFoundException("Employee", dto.Email);
            }

            var passwordHash = new PasswordHasher<string>();
            var verifyPassword = passwordHash.VerifyHashedPassword(dto.Email, existingEmp.PasswordHash, dto.PasswordHash);

            if(verifyPassword == PasswordVerificationResult.Failed)
             {   _logger.LogError(new ArgumentException("Invalid credentials provided."), "LoginAsync: Invalid credentials provided.");
                throw new ForbiddenAccessException("Invalid credentials provided.");
            }

            var accessToken = _token.GenerateAccessToken(existingEmp);
            var (refreshToken, expiresOn) = _token.GenerateRefreshToken();

            _logger.LogInformation($"LoginAsync: User logged in successfully. AccessToken and RefreshToken generated.");

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 15, // min
                TokenType = "Bearer",
                Message = "Login successful."
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogError(new ArgumentNullException(nameof(refreshToken)), "RefreshTokenAsync: Refresh token is required.");
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "RefreshToken", new[] { "Refresh token is required." } }
                });
            }

            var isValid = await ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (!isValid)
            {
                _logger.LogError(new ArgumentException("Invalid or expired refresh token."), "RefreshTokenAsync: Invalid or expired refresh token.");
                throw new ForbiddenAccessException("Invalid or expired refresh token.");
            }
            
            _logger.LogInformation("RefreshTokenAsync: Refresh token is valid.");
            throw new NotImplementedException("Refresh token validation against database needs implementation.");
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;


            await Task.CompletedTask; 
            return false;
        }

    }
}
