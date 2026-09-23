using ECommerce.Application.Common.Caching;
using System.Collections.Concurrent;

namespace ECommerce.IntegrationTests.Fakes;

/// <summary>
/// In-memory implementation of ICacheService for tests.
/// Avoids a Redis dependency during integration testing.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_store.TryGetValue(key, out var value) && value is T typed)
            return Task.FromResult<T?>(typed);

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        _store[key] = value!;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var keys = _store.Keys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keys) _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}