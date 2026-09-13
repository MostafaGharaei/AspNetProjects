using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.WebApi.Middleware;
using ECommerce.WebApi.OpenApi;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// ---------- Layer registrations ----------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- Controllers + FluentValidation ----------
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

// ---------- API Versioning ----------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// ---------- Swagger (with versioning support) ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1",
        Description = "A sample E-Commerce REST API built with ASP.NET Core."
    });
});

var app = builder.Build();

// ---------- Global exception handling middleware ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ---------- Swagger UI ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"ECommerce API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();