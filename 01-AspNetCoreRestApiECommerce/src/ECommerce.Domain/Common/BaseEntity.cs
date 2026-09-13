namespace ECommerce.Domain.Common;

/// <summary>
/// Base entity that all domain entities inherit from.
/// Provides common identity and audit fields.
/// </summary>
/// 
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}