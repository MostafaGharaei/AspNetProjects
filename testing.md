# Testing Strategy

## Test Pyramid

        /\
       /  \      E2E (future)
      /____\
     /      \    Integration Tests  (xUnit + WebApplicationFactory)
    /________\
   /          \  Unit Tests         (xUnit + Moq + FluentAssertions)
  /____________\

## Projects

| Project | Purpose | Tools |
|---|---|---|
| `ECommerce.UnitTests` | Test business logic in isolation | xUnit, Moq, FluentAssertions |
| `ECommerce.IntegrationTests` | Test HTTP pipeline end-to-end | xUnit, WebApplicationFactory, InMemory EF |

## Unit Tests (`ECommerce.UnitTests`)

Location: `Services/ProductServiceTests.cs`

Coverage:
- `GetAllAsync` → mapping + count
- `GetByIdAsync` → found / not found
- `CreateAsync` → Add + SaveChanges called once
- `UpdateAsync` → found / not found
- `DeleteAsync` → found / not found

Repositories and UnitOfWork are mocked with Moq to keep tests fast and deterministic.

## Integration Tests (`ECommerce.IntegrationTests`)

Location: `Controllers/ProductsControllerTests.cs`

- Uses `CustomWebApplicationFactory` to swap SQL Server with EF InMemory.
- Uses a unique DB name per factory instance to isolate test runs.
- Covers:
  - Full CRUD flow (Create → Get → Update → Delete)
  - Validation failures (400)
  - Not found (404)

## Running Tests

```bash
dotnet test