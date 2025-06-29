namespace TierList.Domain.Abstraction;

/// <summary>
/// Represents a generic entity with a unique identifier.
/// </summary>
/// <remarks>This interface is intended to be implemented by classes that represent entities in a system,
/// providing a standard way to access and manage their unique identifiers.</remarks>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    int Id { get; set; }
}
