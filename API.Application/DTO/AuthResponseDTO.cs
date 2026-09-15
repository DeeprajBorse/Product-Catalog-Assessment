using System;

namespace API.Application.DTO
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } // in minutes
        public string TokenType { get; set; } = "Bearer";
        public string Message { get; set; } = string.Empty;
    }
}
