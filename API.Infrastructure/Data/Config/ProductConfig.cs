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
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //Table Name
            builder.ToTable("Products");
            //Primary Key
            builder.HasKey(p => p.Id);
            //Identity Column
            builder.Property(p => p.Id).UseIdentityColumn();

            //Properties
            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.ProductDescription)
                .HasMaxLength(250);
            builder.Property(p => p.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.CreatedOn)
                .IsRequired();
            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(50);
            builder.Property(p => p.UpdatedOn);
            builder.Property(p => p.ProductPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            //Relationships
            builder.HasMany(p => p.Items)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Items_Products");


        }
    }
}
