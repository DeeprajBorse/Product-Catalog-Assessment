using API.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IGenericRepository<Employee> Employees { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}