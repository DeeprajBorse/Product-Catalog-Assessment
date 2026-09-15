using API.Application.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates an employee and returns access and refresh tokens
        /// </summary>
        Task<AuthResponseDTO> LoginAsync(AuthDTO credentials, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes the access token using a valid refresh token
        /// </summary>
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates if a refresh token is still valid
        /// </summary>
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
