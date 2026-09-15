using API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Data.Config
{
    public class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            //Table Name
            builder.ToTable("User");
            //Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Id).UseIdentityColumn(seed:1001, increment:1);

            //Properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Role)
                .IsRequired()
                .HasConversion<string>() 
                .HasMaxLength(50);
        }
    }
}
