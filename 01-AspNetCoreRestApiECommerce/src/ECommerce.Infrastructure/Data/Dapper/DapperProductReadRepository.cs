using Dapper;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Data.Dapper;

/// <summary>
/// Dapper-based read repository.
/// Uses raw SQL for maximum performance on report queries.
/// </summary>
public class DapperProductReadRepository : IProductReadRepository
{
    private readonly DapperContext _context;

    public DapperProductReadRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductReportRow>> GetProductReportAsync(
        int? categoryId = null,
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT  p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    c.Name AS CategoryName
            FROM    Products p
            INNER JOIN Categories c ON c.Id = p.CategoryId
            WHERE   (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
            ORDER BY p.Name";

        using var connection = _context.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { CategoryId = categoryId },
            cancellationToken: ct);

        var result = await connection.QueryAsync<ProductReportRow>(command);
        return result.ToList();
    }

    public async Task<IReadOnlyList<CategoryStockRow>> GetCategoryStockSummaryAsync(
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT  c.Id                                AS CategoryId,
                    c.Name                              AS CategoryName,
                    COUNT(p.Id)                         AS ProductCount,
                    ISNULL(SUM(p.Price * p.Stock), 0)   AS TotalStockValue
            FROM    Categories c
            LEFT JOIN Products p ON p.CategoryId = c.Id
            GROUP BY c.Id, c.Name
            ORDER BY TotalStockValue DESC";

        using var connection = _context.CreateConnection();

        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<CategoryStockRow>(command);
        return result.ToList();
    }
}