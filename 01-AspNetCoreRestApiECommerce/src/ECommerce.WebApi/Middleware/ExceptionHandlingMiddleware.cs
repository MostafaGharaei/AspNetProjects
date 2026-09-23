using ECommerce.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace ECommerce.WebApi.Middleware;

/// <summary>
/// Global exception handler: converts unhandled exceptions
/// into a standard ApiResponse with the correct HTTP status code.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ApiResponse<object>.Fail(
                $"DEBUG → {ex.GetType().Name}: {ex.Message} | Inner: {ex.InnerException?.Message} | Stack: {ex.StackTrace}",
                context.Response.StatusCode);

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}