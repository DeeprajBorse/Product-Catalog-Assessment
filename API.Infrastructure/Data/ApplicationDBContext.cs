using API.Domain.Entities;
using API.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        
        public  DbSet<Item> Items { get; set; } 
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //// Apply configurations from the assembly
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);
            modelBuilder.ApplyConfiguration(new ItemsConfig());
            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new EmployeeConfig());
        }
    }
}
