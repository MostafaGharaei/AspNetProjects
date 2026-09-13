namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Unit of Work abstraction: coordinates repositories and transaction commits.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : Common.BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}