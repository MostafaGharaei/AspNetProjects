using ECommerce.Domain.Common;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace ECommerce.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation that coordinates repositories
/// and manages a single database transaction.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        return (IGenericRepository<TEntity>)_repositories.GetOrAdd(
            typeof(TEntity),
            _ => new GenericRepository<TEntity>(_context));
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _currentTransaction ??= await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
            if (_currentTransaction is not null)
                await _currentTransaction.CommitAsync(ct);
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
            await _currentTransaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}