namespace TierList.Domain.Abstraction;

/// <summary>
/// Represents the base class for aggregate roots in a domain-driven design context.
/// </summary>
/// <remarks>An aggregate root is the main entity that controls access to the aggregate, ensuring that all changes
/// to the aggregate are made through it. This class provides a foundation for implementing aggregate roots, inheriting
/// from the <see cref="Entity"/> class.</remarks>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot()
        : base()
    {
        // Aggregate root constructor logic can be added here if needed.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the aggregate root.</param>
    protected AggregateRoot(int id)
        : base(id)
    {
        // Aggregate root constructor logic can be added here if needed.
    }
}
