using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task AddAsync(T entity, CancellationToken ct = default);

        Task UpdateAsync(T entity, CancellationToken ct = default);

        Task DeleteAsync(T entity, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);

        Task<T> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
    }
}