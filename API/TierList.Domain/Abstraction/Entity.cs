﻿namespace TierList.Domain.Abstraction;

/// <summary>
/// Represents a base class for entities with a unique identifier.
/// </summary>
/// <remarks>The <see cref="Entity"/> class provides a foundation for creating entities with a unique identifier.
/// It is designed to be inherited by other classes that require an identifier property.</remarks>
public abstract class Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// The identifier will be set by the database upon persistence.
    /// </summary>
    protected Entity()
    {
        // Id will be set by the database/ORM
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class with the specified identifier.
    /// This constructor is primarily used for testing or when loading existing entities.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    protected Entity(int id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This value is typically generated by the database.
    /// </summary>
    public int Id { get; protected set; }
}
