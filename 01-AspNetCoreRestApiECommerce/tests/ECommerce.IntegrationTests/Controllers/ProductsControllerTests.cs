using ECommerce.Application.DTOs.Products;
using ECommerce.IntegrationTests.Factories;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace ECommerce.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private const string BaseUrl = "/api/v1/products";

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullCrudFlow_ShouldWorkEndToEnd()
    {
        // ---------- CREATE ----------
        var createDto = new CreateProductDto
        {
            Name = "E2E Keyboard",
            Description = "Mechanical",
            Price = 99.99m,
            Stock = 15,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsJsonAsync(BaseUrl, createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content
            .ReadFromJsonAsync<ApiResponseEnvelope<ProductDto>>();
        created!.Success.Should().BeTrue();
        var id = created.Data!.Id;

        // ---------- READ (single) ----------
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content
            .ReadFromJsonAsync<ApiResponseEnvelope<ProductDto>>();
        fetched!.Data!.Name.Should().Be("E2E Keyboard");

        // ---------- READ (all) ----------
        var allResponse = await _client.GetAsync(BaseUrl);
        allResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var all = await allResponse.Content
            .ReadFromJsonAsync<ApiResponseEnvelope<List<ProductDto>>>();
        all!.Data.Should().Contain(p => p.Id == id);

        // ---------- UPDATE ----------
        var updateDto = new UpdateProductDto
        {
            Name = "E2E Keyboard v2",
            Description = "Updated",
            Price = 109.99m,
            Stock = 10,
            CategoryId = 1
        };

        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterUpdate = await _client.GetAsync($"{BaseUrl}/{id}");
        var updated = await afterUpdate.Content
            .ReadFromJsonAsync<ApiResponseEnvelope<ProductDto>>();
        updated!.Data!.Name.Should().Be("E2E Keyboard v2");
        updated.Data.Price.Should().Be(109.99m);

        // ---------- DELETE ----------
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterDelete = await _client.GetAsync($"{BaseUrl}/{id}");
        afterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithInvalidPayload_ShouldReturn400()
    {
        // Name empty + Price <= 0 + Stock negative → FluentValidation kicks in
        var invalid = new CreateProductDto
        {
            Name = "",
            Price = -5m,
            Stock = -1,
            CategoryId = 0
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync($"{BaseUrl}/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class ApiResponseEnvelope<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
}