using API.Application.Interfaces;
using API.Domain.Options;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Repositories;
using API.Infrastructure.Identity;
using API.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace API.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration config)
        {
            // Register configuration options to fetch connection string 
            services.AddDbContext<ApplicationDBContext>((serviceprovider, options) =>
            options.UseSqlServer(serviceprovider.GetRequiredService<IOptionsMonitor<ConnectionstringOptions>>().CurrentValue.ConnectString));

            // Register repositories and unit of work
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Identity/JWT service
            services.AddScoped<IGenerateJWTToken, GenerateJWTToken>();

            //Registering Logger adapter
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

            //Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            return services;
        }
    }
}
