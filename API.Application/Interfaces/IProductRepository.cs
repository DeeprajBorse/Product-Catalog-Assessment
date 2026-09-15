using API.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithItemsAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string productName, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<Product?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetByAsync(Func<Product, bool> value, CancellationToken cancellationToken = default);
    }
}