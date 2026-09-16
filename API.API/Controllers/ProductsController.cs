using API.Application.DTO;
using API.Application.Interfaces;
using API.Domain.Enum;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Authorize] 
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)},{nameof(Roles.User)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)},{nameof(Roles.User)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Retrieves a product by its name.
        /// </summary>
        [HttpGet("by-name/{productName}")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)},{nameof(Roles.User)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByProductName([FromRoute] string productName, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByProductNameAsync(productName, cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Retrieves a list of items associated with a specific product by its unique identifier.
        /// </summary>
        [HttpGet("{productId:int}/items")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)},{nameof(Roles.User)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetItemsByProductId([FromRoute] int productId, CancellationToken cancellationToken = default)
        {
            var items = await _productService.GetItemsByProductIdAsync(productId, cancellationToken);
            return Ok(items);
        }

        /// <summary>
        /// Creates a new product along with associated items.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)}")] 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto, CancellationToken cancellationToken = default)
        {
            var createdProduct = await _productService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id, version = "1.0" }, createdProduct);
        }

        /// <summary>
        /// Updates an existing product by its unique identifier.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDTO dto, CancellationToken cancellationToken = default)
        {
            var updateProduct = await _productService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updateProduct);
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(Roles.Admin))] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}