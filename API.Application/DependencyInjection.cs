using API.Application.AutoMapper;
using API.Application.Interfaces;
using API.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace API.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            //DI for FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Register AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperConfig>());

            return services;
        }
    }
}
