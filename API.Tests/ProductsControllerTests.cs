using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.Application.DTO;
using API.Application.DTOs;
using FluentAssertions;
using Xunit;

namespace API.Tests
{
    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_WithoutToken_ReturnsUnauthorized()
        {
            // Clear authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/api/v1/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task HealthCheck_ReturnsSuccess()
        {

            var response = await _client.GetAsync("/health");

            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task CreateProduct_WithoutToken_ReturnsUnauthorized()
        {
            // Clear authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            var newProduct = new CreateProductDTO
            {
                ProductName = "Unauthorized Test Product",
                ProductDescription = "No Token Provided",
                ProductPrice = 99.99m,
                CreatedBy = "Tester"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/products", newProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateProduct_WithValidToken_ReturnsCreated()
        {
            // Arrange
            var token = AuthTokenHelper.GenerateToken(role: "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newProduct = new CreateProductDTO
            {
                ProductName = $"Integration Test Product {Guid.NewGuid():N}",
                ProductDescription = "Created through integration test",
                ProductPrice = 149.99m,
                CreatedBy = "IntegrationTester",
                Items = new List<CreateItemDTO>
                {
                    new() { Quantity = 5 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/products", newProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<ProductDTO>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.ProductName.Should().Be(newProduct.ProductName);
        }

        [Fact]
        public async Task GetAllProducts_WithValidToken_ReturnsOk()
        {
            // Arrange
            var token = AuthTokenHelper.GenerateToken(role: "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }
}