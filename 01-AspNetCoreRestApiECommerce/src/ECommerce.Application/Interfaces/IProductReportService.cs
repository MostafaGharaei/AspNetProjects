using ECommerce.Application.DTOs.Reports;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Application service that exposes report queries
/// (backed by Dapper for performance).
/// </summary>
public interface IProductReportService
{
    Task<IReadOnlyList<ProductReportDto>> GetProductReportAsync(
        int? categoryId = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<CategoryStockDto>> GetCategoryStockSummaryAsync(
        CancellationToken ct = default);
}