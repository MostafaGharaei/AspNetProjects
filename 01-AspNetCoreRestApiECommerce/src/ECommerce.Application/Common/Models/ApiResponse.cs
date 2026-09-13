namespace ECommerce.Application.Common.Models;

/// <summary>
/// Standard API response wrapper for all endpoints.
/// Ensures a consistent contract for clients (success/errors/data).
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        StatusCode = 200
    };

    public static ApiResponse<T> Created(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        StatusCode = 201
    };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors,
        StatusCode = statusCode
    };
}