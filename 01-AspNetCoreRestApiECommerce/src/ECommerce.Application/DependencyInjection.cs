using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

/// <summary>
/// Registers Application layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}