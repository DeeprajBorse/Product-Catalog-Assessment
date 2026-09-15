using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace API.Middleware
{
    /// <summary>
    /// Middleware to add security headers to all HTTP responses
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AddSecurityHeaders(context.Response.Headers);
            await _next(context);
        }

        private void AddSecurityHeaders(IHeaderDictionary headers)
        {
            try
            {
                // HSTS (HTTP Strict Transport Security) - Force HTTPS for 1 year
                headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

                // Prevent clickjacking attacks
                headers.Add("X-Frame-Options", "DENY");

                // Prevent MIME type sniffing
                headers.Add("X-Content-Type-Options", "nosniff");

                // Enable XSS protection in older browsers
                headers.Add("X-XSS-Protection", "1; mode=block");

                // Referrer Policy - Limit referrer information
                headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

                // Content Security Policy - Restrict resource loading
                headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self'; " +
                    "connect-src 'self'; " +
                    "frame-ancestors 'none'");

                // Feature Policy / Permissions Policy - Restrict browser features
                headers.Add("Permissions-Policy",
                    "accelerometer=(), " +
                    "camera=(), " +
                    "geolocation=(), " +
                    "gyroscope=(), " +
                    "magnetometer=(), " +
                    "microphone=(), " +
                    "payment=(), " +
                    "usb=()");

                // Remove Server header to not disclose server info
                headers.Remove("Server");

                _logger.LogDebug("Security headers added to response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding security headers");
            }
        }
    }

    /// <summary>
    /// Extension method for adding security headers middleware
    /// </summary>
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
