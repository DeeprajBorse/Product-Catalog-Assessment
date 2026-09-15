using API.Application.AutoMapper;
using API.Application.DTO;
using API.Application.Interfaces;
using API.Application.Services;
using API.Domain.Entities;
using API.Domain.Exceptions;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Timers;
using Xunit;
using ValidationException = API.Domain.Exceptions.ValidationException;

namespace Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IAppLogger<ProductService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly ProductService _sut;
        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
            _loggerMock = new Mock<IAppLogger<ProductService>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperConfig>(), NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _sut = new ProductService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedDto_WhenProductExists()
        {
            var product = new Product { Id = 1, ProductName = "Widget", ProductDescription = "A widget", ProductPrice = 9.99m, CreatedBy = "tester", CreatedOn = System.DateTime.UtcNow };
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var result = await _sut.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ProductName.Should().Be("Widget");
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenProductDoesNotExist()
        {
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            Func<Task> act = async () => await _sut.GetByIdAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_AddsProductAndSaves_WhenNameIsUnique()
        {
            var dto = new CreateProductDTO
            {
                ProductName = "New Product",
                ProductDescription = "Description",
                ProductPrice = 19.99m,
                CreatedBy = "tester"
            };

            _productRepositoryMock
                .Setup(r => r.ExistsByNameAsync(dto.ProductName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.CreateAsync(dto);

            result.ProductName.Should().Be(dto.ProductName);
            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenProductNameAlreadyExists()
        {
            var dto = new CreateProductDTO { ProductName = "Duplicate", CreatedBy = "tester" };

            _productRepositoryMock
                .Setup(r => r.ExistsByNameAsync(dto.ProductName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            Func<Task> act = async () => await _sut.CreateAsync(dto);

            await act.Should().ThrowAsync<ValidationException>();
            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenProductDoesNotExist()
        {
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            Func<Task> act = async () => await _sut.DeleteAsync(42);

            await act.Should().ThrowAsync<NotFoundException>();
            _productRepositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndSaves_WhenProductExists()
        {
            var product = new Product { Id = 5, ProductName = "ToDelete" };
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            await _sut.DeleteAsync(5);

            _productRepositoryMock.Verify(r => r.Delete(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}