namespace ECommerce.Infrastructure.Caching;

/// <summary>
/// Centralized cache key factory to prevent key collisions and typos.
/// </summary>
public static class CacheKeys
{
    public static string ProductById(int id) => $"product:{id}";
    public static string AllProducts() => "products:all";
    public static string ProductReport(int? categoryId) =>
        categoryId.HasValue ? $"report:products:cat:{categoryId}" : "report:products:all";
    public static string CategoryStockSummary() => "report:category-stock";
}