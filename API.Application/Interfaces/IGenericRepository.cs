using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<T?> GetByAsync(
            Expression<Func<T, bool>> filter,
            bool useNoTracking = true,
            CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        // Staging operations only — no I/O happens until IUnitOfWork.SaveChangesAsync is called
        void Update(T entity);

        void Delete(T entity);
    }
}