using ECommerce.Application.DTOs.Products;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Application service contract for product operations.
/// </summary>
public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}