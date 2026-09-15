using API.Application.DTO;
using API.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDTO>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<ProductDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductDTO> GetByProductNameAsync(string productName, CancellationToken cancellationToken = default);
        Task<IEnumerable<ItemDTO>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductDTO> CreateAsync(CreateProductDTO dto, CancellationToken cancellationToken = default);
        Task<ProductDTO> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
