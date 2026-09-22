namespace ECommerce.Application.DTOs.Reports;

/// <summary>
/// Report DTO for product listing with category info.
/// </summary>
public class ProductReportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}