using API.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainDI(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ConnectionstringOptions>(config.GetSection(ConnectionstringOptions.SectionName));
            return services;
        }
    }
}
