namespace API.API
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var corSettings = configuration.GetSection("Cors");
            var allowedOrigins = corSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:3000" };
            var allowedMethods = corSettings.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
            var allowedHeaders = corSettings.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "*" };

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .WithMethods(allowedMethods)
                        .WithHeaders(allowedHeaders)
                        .AllowCredentials()
                        .WithExposedHeaders("X-Pagination", "X-Total-Count", "Token-Expired");
                });
            });

            return services;
        }
    }
}
