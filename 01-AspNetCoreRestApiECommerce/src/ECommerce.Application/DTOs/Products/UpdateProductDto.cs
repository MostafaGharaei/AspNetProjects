namespace ECommerce.Application.DTOs.Products;

/// <summary>
/// DTO for updating an existing product.
/// </summary>
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
}