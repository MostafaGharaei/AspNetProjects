using ECommerce.Application.Common.Caching;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

/// <summary>
/// Product service implementation.
/// Uses Repository + Unit of Work for writes,
/// and a cache layer for reads to improve performance.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    // Cache TTL for product reads
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public ProductService(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        // 1. Try cache first
        var cached = await _cache.GetAsync<List<ProductDto>>(CacheKeys.AllProducts(), ct);
        if (cached is not null)
            return cached;

        // 2. Cache miss → hit DB
        var repo = _uow.Repository<Product>();
        var products = await repo.GetAllAsync(ct);
        var dtos = products.Select(MapToDto).ToList();

        // 3. Store in cache
        await _cache.SetAsync(CacheKeys.AllProducts(), dtos, CacheTtl, ct);

        return dtos;
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<ProductDto>(CacheKeys.ProductById(id), ct);
        if (cached is not null)
            return cached;

        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        if (product is null) return null;

        var dto = MapToDto(product);
        await _cache.SetAsync(CacheKeys.ProductById(id), dto, CacheTtl, ct);

        return dto;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        await repo.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        // Invalidate list cache (new product added)
        await _cache.RemoveAsync(CacheKeys.AllProducts(), ct);

        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        if (product is null) return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        repo.Update(product);
        await _uow.SaveChangesAsync(ct);

        // Invalidate both the single-item cache and the list cache
        await _cache.RemoveAsync(CacheKeys.ProductById(id), ct);
        await _cache.RemoveAsync(CacheKeys.AllProducts(), ct);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        if (product is null) return false;

        repo.Remove(product);
        await _uow.SaveChangesAsync(ct);

        // Invalidate caches
        await _cache.RemoveAsync(CacheKeys.ProductById(id), ct);
        await _cache.RemoveAsync(CacheKeys.AllProducts(), ct);

        return true;
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name
    };
}