using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

/// <summary>
/// Product service implementation.
/// Uses Repository + Unit of Work patterns to keep persistence concerns out of the API layer.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var products = await repo.GetAllAsync(ct);

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        await repo.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        if (product is null) return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        repo.Update(product);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Product>();
        var product = await repo.GetByIdAsync(id, ct);
        if (product is null) return false;

        repo.Remove(product);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name
    };
}