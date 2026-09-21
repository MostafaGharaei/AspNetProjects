using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

/// <summary>
/// Unit tests for ProductService using mocked IUnitOfWork and IGenericRepository.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IGenericRepository<Product>> _productRepoMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        // Repository<Product>() always returns the same mocked repo
        _uowMock.Setup(u => u.Repository<Product>()).Returns(_productRepoMock.Object);
        _sut = new ProductService(_uowMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 1500m, Stock = 5, CategoryId = 1 },
            new() { Id = 2, Name = "Phone",  Price = 800m,  Stock = 10, CategoryId = 1 }
        };
        _productRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Laptop");
        result[1].Name.Should().Be("Phone");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnDto()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 1500m, Stock = 5, CategoryId = 1 };
        _productRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAndSaveAndReturnDto()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Tablet",
            Price = 500m,
            Stock = 20,
            CategoryId = 2
        };

        _productRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<Product, CancellationToken>((p, _) => p.Id = 42);

        _uowMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Tablet");
        result.Price.Should().Be(500m);

        _productRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _uowMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_ShouldUpdateAndSave()
    {
        // Arrange
        var existing = new Product { Id = 1, Name = "Old", Price = 100m, Stock = 1, CategoryId = 1 };
        _productRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _uowMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var dto = new UpdateProductDto
        {
            Name = "New",
            Price = 200m,
            Stock = 3,
            CategoryId = 2
        };

        // Act
        var result = await _sut.UpdateAsync(1, dto);

        // Assert
        result.Should().BeTrue();
        existing.Name.Should().Be("New");
        existing.Price.Should().Be(200m);

        _productRepoMock.Verify(r => r.Update(existing), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var dto = new UpdateProductDto { Name = "X", Price = 1m, Stock = 1, CategoryId = 1 };

        // Act
        var result = await _sut.UpdateAsync(99, dto);

        // Assert
        result.Should().BeFalse();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldRemoveAndSave()
    {
        // Arrange
        var existing = new Product { Id = 1, Name = "Old", Price = 100m, Stock = 1, CategoryId = 1 };
        _productRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _uowMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _productRepoMock.Verify(r => r.Remove(existing), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.DeleteAsync(99);

        // Assert
        result.Should().BeFalse();
        _productRepoMock.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
    }
}