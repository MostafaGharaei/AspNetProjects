namespace ECommerce.Application.DTOs.Products;

/// <summary>
/// DTO for creating a new product.
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
}