using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Read-only repository using Dapper for high-performance queries.
/// Kept separate from the write repository (CQRS-lite) to allow
/// optimized SQL for reports without polluting the write model.
/// </summary>
public interface IProductReadRepository
{
    /// <summary>
    /// Returns products with their category name, filtered optionally by category.
    /// </summary>
    Task<IReadOnlyList<ProductReportRow>> GetProductReportAsync(
        int? categoryId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Returns total stock value grouped by category.
    /// </summary>
    Task<IReadOnlyList<CategoryStockRow>> GetCategoryStockSummaryAsync(
        CancellationToken ct = default);
}

/// <summary>
/// Flat row returned by Dapper for product reports.
/// </summary>
public class ProductReportRow
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

/// <summary>
/// Flat row returned by Dapper for category stock summary.
/// </summary>
public class CategoryStockRow
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalStockValue { get; set; }
}