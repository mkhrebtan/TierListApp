using TierList.Domain.Abstraction;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents an image entity associated with a specific tier, including its metadata and relationship to a container.
/// </summary>
/// <remarks>This entity is typically used to store information about images, such as their storage key, display order,
/// and optional notes. It also includes a reference to the container that groups related images.</remarks>
public class TierImageEntity : Entity
{
    public const int MaxNoteLength = 1000;

    private TierImageEntity(Guid storageKey, string url, int containerId, Order order)
        : base()
    {
        StorageKey = storageKey;
        Url = url;
        Note = string.Empty;
        ContainerId = containerId;
        Order = order;
    }

    /// <summary>
    /// Gets the unique identifier used as the storage key.
    /// </summary>
    public Guid StorageKey { get; private set; }

    /// <summary>
    /// Gets the URL associated with this instance.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Gets the order in which this item should be processed or displayed.
    /// </summary>
    public Order Order { get; private set; }

    /// <summary>
    /// Gets a note or comment associated with the object.
    /// </summary>
    public string Note { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the container that holds image data.
    /// </summary>
    public int ContainerId { get; private set; }

    internal static Result<TierImageEntity> Create(Guid storageKey, string url, int containerId, Order order)
    {
        if (storageKey == Guid.Empty)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.EmptyStorageKey", "Storage key cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.EmptyUrl", "Image URL cannot be null or empty."));
        }

        if (containerId <= 0)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.InvalidContainerId", "Container ID must be a positive integer."));
        }

        return Result<TierImageEntity>.Success(new TierImageEntity(storageKey, url, containerId, order));
    }

    internal void UpdateOrder(Order newOrder)
    {
        Order = newOrder;
    }

    internal Result UpdateContainer(int containerId)
    {
        if (containerId <= 0)
        {
            return Result.Failure(new Error("TierImage.InvalidContainerId", "Container ID must be a positive integer."));
        }

        ContainerId = containerId;
        return Result.Success();
    }

    internal Result UpdateNote(string note)
    {
        if (note.Length > MaxNoteLength)
        {
            return Result.Failure(new Error("TierImage.NoteTooLong", $"Note cannot exceed {MaxNoteLength} characters."));
        }

        Note = note;
        return Result.Success();
    }

    internal Result UpdateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure(new Error("TierImage.EmptyUrl", "Image URL cannot be null or empty."));
        }

        Url = url;
        return Result.Success();
    }
}