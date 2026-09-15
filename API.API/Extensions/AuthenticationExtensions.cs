using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;

namespace API.API
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT");
            var secret = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(secret) || Encoding.UTF8.GetBytes(secret).Length < 16)
            {
                throw new InvalidOperationException("JWT secret is missing or too short (minimum 16 bytes).");
            }

            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT Issuer and Audience are required.");
            }

            var key = Encoding.UTF8.GetBytes(secret);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        // Critical for role-based authorization:
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync(new ErrorResponse
                            {
                                StatusCode = StatusCodes.Status401Unauthorized,
                                Message = "Unauthorized: Invalid or missing authentication token."
                            }.ToString());
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync(new ErrorResponse
                            {
                                StatusCode = StatusCodes.Status403Forbidden,
                                Message = "Forbidden: You do not have permission to access this resource."
                            }.ToString());
                        }
                    };
                });

            return services;
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}