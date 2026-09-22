using ECommerce.Application.Common.Caching;
using ECommerce.Application.DTOs.Reports;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

/// <summary>
/// Report service backed by Dapper.
/// Caches report results for short periods (reports change less often).
/// </summary>
public class ProductReportService : IProductReportService
{
    private readonly IProductReadRepository _readRepo;
    private readonly ICacheService _cache;

    private static readonly TimeSpan ReportTtl = TimeSpan.FromMinutes(2);

    public ProductReportService(IProductReadRepository readRepo, ICacheService cache)
    {
        _readRepo = readRepo;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ProductReportDto>> GetProductReportAsync(
        int? categoryId = null,
        CancellationToken ct = default)
    {
        var key = CacheKeys.ProductReport(categoryId);

        var cached = await _cache.GetAsync<List<ProductReportDto>>(key, ct);
        if (cached is not null) return cached;

        var rows = await _readRepo.GetProductReportAsync(categoryId, ct);

        var dtos = rows.Select(r => new ProductReportDto
        {
            Id = r.Id,
            Name = r.Name,
            Price = r.Price,
            Stock = r.Stock,
            CategoryName = r.CategoryName
        }).ToList();

        await _cache.SetAsync(key, dtos, ReportTtl, ct);
        return dtos;
    }

    public async Task<IReadOnlyList<CategoryStockDto>> GetCategoryStockSummaryAsync(
        CancellationToken ct = default)
    {
        var key = CacheKeys.CategoryStockSummary();

        var cached = await _cache.GetAsync<List<CategoryStockDto>>(key, ct);
        if (cached is not null) return cached;

        var rows = await _readRepo.GetCategoryStockSummaryAsync(ct);

        var dtos = rows.Select(r => new CategoryStockDto
        {
            CategoryId = r.CategoryId,
            CategoryName = r.CategoryName,
            ProductCount = r.ProductCount,
            TotalStockValue = r.TotalStockValue
        }).ToList();

        await _cache.SetAsync(key, dtos, ReportTtl, ct);
        return dtos;
    }
}