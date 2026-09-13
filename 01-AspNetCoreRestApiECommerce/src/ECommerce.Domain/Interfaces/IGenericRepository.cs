using ECommerce.Domain.Common;
using System.Linq.Expressions;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Generic repository abstraction (Repository Pattern).
/// Lives in Domain so higher layers depend on abstractions, not implementations.
/// </summary>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}