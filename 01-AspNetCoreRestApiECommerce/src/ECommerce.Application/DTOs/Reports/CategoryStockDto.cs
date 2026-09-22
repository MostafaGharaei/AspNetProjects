namespace ECommerce.Application.DTOs.Reports;

/// <summary>
/// Report DTO for aggregated stock value per category.
/// </summary>
public class CategoryStockDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalStockValue { get; set; }
}