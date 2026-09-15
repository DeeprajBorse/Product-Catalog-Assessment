using API.Application.Interfaces;
using API.Domain.Entities;
using API.Infrastructure.Data.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace API.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        private IProductRepository? _products;
        private IGenericRepository<Employee>? _emp;
        private bool _disposed;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);

        public IGenericRepository<Employee> Employees => _emp ??= new GenericRepository<Employee>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}