# ECommerce REST API

A sample e-commerce REST API built with ASP.NET Core, following Clean Architecture principles.

## Tech Stack
- .NET 9 / ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- FluentValidation
- API Versioning (Asp.Versioning)
- Swagger / OpenAPI

## Architecture
Layered (Clean Architecture ready):
- `ECommerce.Domain` — Entities, abstractions
- `ECommerce.Application` — DTOs, services, validators
- `ECommerce.Infrastructure` — EF Core, Repository, Unit of Work
- `ECommerce.WebApi` — Controllers, middleware, DI

## Design Patterns Used
- Repository
- Unit of Work
- Dependency Injection
- Options / Middleware

## Endpoints (v1)
| Method | Route | Description |
|---|---|---|
| GET | /api/v1/products | Get all products |
| GET | /api/v1/products/{id} | Get product by id |
| POST | /api/v1/products | Create product |
| PUT | /api/v1/products/{id} | Update product |
| DELETE | /api/v1/products/{id} | Delete product |

## How to Run
```bash
dotnet restore
dotnet ef database update -p src/ECommerce.Infrastructure -s src/ECommerce.WebApi
dotnet run --project src/ECommerce.WebApi