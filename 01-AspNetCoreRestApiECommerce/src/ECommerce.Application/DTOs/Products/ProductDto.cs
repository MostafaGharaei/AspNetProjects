namespace ECommerce.Application.DTOs.Products;

/// <summary>
/// Data Transfer Object for returning product data to clients.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

