# Data Access Strategy

## Overview

We use two data-access technologies side by side:

| Concern | Technology | Why |
|---|---|---|
| Writes (CRUD) | EF Core | Change tracking, migrations, LINQ |
| Reads (Reports) | Dapper | Raw SQL, minimal overhead, maximum control |
| Caching | Redis | Distributed cache, shared across instances |

This is a lightweight **CQRS** approach: the write model and read model are separate code paths, but share the same database.

## When to use what

### EF Core (`IGenericRepository<T>` + `IUnitOfWork`)
- Create / Update / Delete operations
- Anything that benefits from change tracking
- Simple reads where LINQ is convenient

### Dapper (`IProductReadRepository`)
- Complex report queries (joins, aggregations)
- High-throughput read endpoints
- Queries where you want full control over SQL

## Connection Management

- **EF Core** manages its own connection pool via `DbContext`.
- **Dapper** uses `DapperContext.CreateConnection()` which returns a new `SqlConnection`. 
  - `SqlConnection` is **pooled by ADO.NET** under the hood.
  - Do **not** keep connections open between calls.
  - Always use `using var connection = ...`.

## Query Examples

### Product report (Dapper)

```sql
SELECT  p.Id, p.Name, p.Price, p.Stock, c.Name AS CategoryName
FROM    Products p
INNER JOIN Categories c ON c.Id = p.CategoryId
WHERE   (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
ORDER BY p.Name