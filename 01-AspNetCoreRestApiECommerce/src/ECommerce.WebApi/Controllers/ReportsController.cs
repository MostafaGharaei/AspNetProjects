using Asp.Versioning;
using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Reports;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers;

/// <summary>
/// Report endpoints backed by Dapper (optimized read queries).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IProductReportService _reportService;

    public ReportsController(IProductReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: api/v1/reports/products
    [HttpGet("products")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductReport(
        [FromQuery] int? categoryId,
        CancellationToken ct)
    {
        var result = await _reportService.GetProductReportAsync(categoryId, ct);
        return Ok(ApiResponse<IReadOnlyList<ProductReportDto>>.Ok(result));
    }

    // GET: api/v1/reports/category-stock
    [HttpGet("category-stock")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CategoryStockDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryStockSummary(CancellationToken ct)
    {
        var result = await _reportService.GetCategoryStockSummaryAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<CategoryStockDto>>.Ok(result));
    }
}