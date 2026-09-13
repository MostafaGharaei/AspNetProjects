using Asp.Versioning;
using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers;

/// <summary>
/// Products CRUD endpoints.
/// Route: /api/v1/products
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/v1/products
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await _productService.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<ProductDto>>.Ok(products));
    }

    // GET: api/v1/products/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var product = await _productService.GetByIdAsync(id, ct);
        if (product is null)
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found.", 404));

        return Ok(ApiResponse<ProductDto>.Ok(product));
    }

    // POST: api/v1/products
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var created = await _productService.CreateAsync(dto, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { version = "1", id = created.Id },
            ApiResponse<ProductDto>.Created(created));
    }

    // PUT: api/v1/products/5
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var updated = await _productService.UpdateAsync(id, dto, ct);
        if (!updated)
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found.", 404));

        return NoContent();
    }

    // DELETE: api/v1/products/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _productService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found.", 404));

        return NoContent();
    }
}