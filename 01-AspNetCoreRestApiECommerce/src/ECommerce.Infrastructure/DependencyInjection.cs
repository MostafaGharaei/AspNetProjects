using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Data.Dapper;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

/// <summary>
/// Registers Infrastructure layer services (EF Core, Repositories, Unit of Work).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Distributed cache registration:
        // - Use Redis when `Redis:Configuration` is present in configuration.
        // - Fall back to in-memory distributed cache for local/dev runs.
        var redisConfig = configuration["Redis:Configuration"];
        if (!string.IsNullOrWhiteSpace(redisConfig))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                // Optional: prefix all keys so they are easier to find in redis-cli
                options.InstanceName = "ECommerce:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddSingleton<ICacheService, RedisCacheService>();

        // Dapper context + read repository (for report queries)
        services.AddSingleton<DapperContext>();
        services.AddScoped<IProductReadRepository, DapperProductReadRepository>();

        return services;
    }
}