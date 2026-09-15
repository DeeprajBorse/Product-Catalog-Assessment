using API.Application.Interfaces;
using API.Domain.Entities;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Infrastructure.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsWithItemsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Items)  
                .ToListAsync(cancellationToken);
        }
       
        public async Task<bool> ExistsByNameAsync(string productName, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(p => p.ProductName == productName, cancellationToken);
        }
        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().OrderBy(p => p.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Product?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByAsync(Func<Product, bool> value, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => _dbSet.AsNoTracking().FirstOrDefault(value), cancellationToken);
        }
    }
}