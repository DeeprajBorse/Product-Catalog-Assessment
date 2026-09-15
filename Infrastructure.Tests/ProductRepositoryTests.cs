using API.Domain.Entities;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests
{
    public class ProductRepositoryTests
    {
        private ApplicationDBContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            return new ApplicationDBContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistProductToDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repo = new ProductRepository(context);
            var product = new Product
            {
                ProductName = "Monitor",
                ProductPrice = 199.99m,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            await repo.AddAsync(product);
            await context.SaveChangesAsync();

            // Assert
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Monitor");
            savedProduct.Should().NotBeNull();
            savedProduct!.ProductPrice.Should().Be(199.99m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductName = "Keyboard",
                ProductPrice = 49.99m,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);

            // Act
            var result = await repo.GetByIdAsync(product.Id);

            // Assert
            result.Should().NotBeNull();
            result!.ProductName.Should().Be("Keyboard");
        }

        [Fact]
        public async Task Delete_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product { ProductName = "Headphones", CreatedBy = "Admin", CreatedOn = DateTime.UtcNow };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);

            // Act
            repo.Delete(product);
            await context.SaveChangesAsync();

            // Assert
            var exists = await context.Products.AnyAsync(p => p.Id == product.Id);
            exists.Should().BeFalse();
        }
    }
}