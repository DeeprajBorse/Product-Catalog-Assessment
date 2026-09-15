using API.API.Filters;
using API.Application;
using API.Domain;
using API.Infrastructure;

namespace API.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIDI(this IServiceCollection services, IConfiguration config)
        {
            services.AddDomainDI(config);
            services.AddApplicationDI();
            services.AddInfrastructureDI(config);
            services.AddScoped(typeof(ValidationFilter<>));
            return services;
        }
    }
}
