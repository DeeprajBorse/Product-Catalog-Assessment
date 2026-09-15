using API.Application.DTO;
using API.Application.DTOs;
using API.Application.Interfaces;
using API.Domain.Entities;
using API.Domain.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;


namespace API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProductService> _logger;

        public ProductService(IUnitOfWork unit, IMapper mapper, IAppLogger<ProductService> logger)
        {
            _unit = unit;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<ProductDTO>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var (products, totalCount) = await _unit.Products.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            var result = _mapper.Map<IEnumerable<ProductDTO>>(products);

            _logger.LogInformation($"Retrieving products - Page: {pageNumber}, Size: {pageSize}, TotalCount: {totalCount}");

            return new PagedResult<ProductDTO>
            {
                Items = result.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _unit.Products.GetByIdAsync(id, cancellationToken);

            if (product == null)
                throw new NotFoundException("Product", id);

            _logger.LogInformation($"Retrieving product by ID: {product.Id}");

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> GetByProductNameAsync(string productName, CancellationToken cancellationToken = default)
        {
            var product = await _unit.Products.GetByAsync(x => x.ProductName == productName, cancellationToken: cancellationToken);

            if (product == null)
                throw new NotFoundException("Product", productName);

            _logger.LogInformation($"Retrieving product by name: {product.ProductName}");

            return _mapper.Map<ProductDTO>(product);
        }

        //For ITEM
        public async Task<IEnumerable<ItemDTO>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _unit.Products.GetByIdWithItemsAsync(productId, cancellationToken);

            if (product == null)
                throw new NotFoundException("Product", productId);

            _logger.LogInformation($"Retrieving items for product: {product.ProductName}");

            return _mapper.Map<IEnumerable<ItemDTO>>(product.Items);
        }

        public async Task<ProductDTO> CreateAsync(CreateProductDTO dto, CancellationToken cancellationToken = default)
        {
            if(dto == null)
                throw new ArgumentNullException(nameof(dto));

            if(await _unit.Products.ExistsByNameAsync(dto.ProductName, cancellationToken))
                throw new ValidationException(new Dictionary<string, string[]> 
                { 
                    { "ProductName", new[] { $"Product with name '{dto.ProductName}' already exists." } } 
                });

            var product = _mapper.Map<Product>(dto);
            product.CreatedOn = DateTime.UtcNow;

            await _unit.Products.AddAsync(product, cancellationToken);

            _logger.LogInformation($"Creating a new product: {product.ProductName} by {product.CreatedBy}");

            await _unit.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var product = await _unit.Products.GetByIdAsync(id, cancellationToken);

            if (product == null)
                throw new NotFoundException("Product", id);

            _mapper.Map(dto, product);
            product.UpdatedOn = DateTime.UtcNow;

            _unit.Products.Update(product);

            _logger.LogInformation($"Updating product: {product.ProductName} by {product.UpdatedBy}");

            await _unit.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _unit.Products.GetByIdAsync(id, cancellationToken);

            if (product == null)
                throw new NotFoundException("Product", id);

            _unit.Products.Delete(product);

            _logger.LogInformation($"Deleting product: {product.ProductName} by {product.UpdatedBy}");

            await _unit.SaveChangesAsync(cancellationToken);
        }

    }
}
