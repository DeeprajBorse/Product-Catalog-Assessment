using API.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string TestDbName = "ProductCatalogTestDb";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {

                    ["JWT:SecretKey"] = "Test-Only-Secret-Key-That-Is-At-Least-32-Bytes-Long!",
                    ["JWT:Issuer"] = "TestIssuer",
                    ["JWT:Audience"] = "TestAudience",
                    ["JWT:AccessTokenExpirationMinutes"] = "15",
                    ["JWT:RefreshTokenExpirationDays"] = "7",

                    ["JWT:Key"] = "Test-Only-Secret-Key-That-Is-At-Least-32-Bytes-Long!",
                    
                });
            });

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDBContext>(options =>
                    options.UseInMemoryDatabase(TestDbName));

                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                });
            });
        }
    }
}