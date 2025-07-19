using System.Text.RegularExpressions;
using TierList.Domain.Abstraction;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Domain.Entities;

public class TierRowEntity : TierImageContainer
{
    /// <summary>
    /// Represents the maximum allowable length for a rank.
    /// </summary>
    public const int MaxRankLength = 50;

    private TierRowEntity(int tierListId, string rank, string colorHex, Order order)
        : base(tierListId)
    {
        Rank = rank;
        ColorHex = colorHex;
        Order = order;
    }

    /// <summary>
    /// Gets the rank of the tier row.
    /// </summary>
    public string Rank { get; private set; }

    /// <summary>
    /// Gets the hexadecimal color code representing the color.
    /// </summary>
    public string ColorHex { get; private set; }

    /// <summary>
    /// Gets the order in which this item should be processed or displayed.
    /// </summary>
    public Order Order { get; private set; }

    internal static Result<TierRowEntity> Create(int listId, string rank, string colorHex, Order order)
    {
        if (listId <= 0)
        {
            return Result<TierRowEntity>.Failure(new Error("TierRow.InvalidListId", "List ID must be greater than zero."));
        }

        if (string.IsNullOrWhiteSpace(rank))
        {
            return Result<TierRowEntity>.Failure(new Error("TierRow.EmptyRank", "Rank cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(colorHex) || !Regex.IsMatch(colorHex, @"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$"))
        {
            return Result<TierRowEntity>.Failure(new Error("TierRow.InvalidColorHex", "ColorHex must be a valid hexadecimal color code."));
        }

        return Result<TierRowEntity>.Success(new TierRowEntity(listId, rank, colorHex, order));
    }

    /// <summary>
    /// Increments the current order value by one.
    /// </summary>
    /// <remarks>This method increases the value of the <c>Order</c> property by one each time it is called.
    /// Ensure that the <c>Order</c> property is initialized before calling this method to avoid unexpected
    /// results.</remarks>
    internal void IncrementOrder()
    {
        Order = Order.Increment();
    }

    /// <summary>
    /// Decreases the order value by one, if the current order is greater than one.
    /// </summary>
    /// <remarks>This method ensures that the order value does not fall below one. It is typically used to
    /// adjust the order in a sequence where a minimum order of one is required.</remarks>
    internal void DecrementOrder()
    {
        if (Order > 1)
        {
            Order = Order.Decrement().Value;
        }
    }

    internal void UpdateOrder(Order order)
    {
        Order = order;
    }

    internal Result UpdateColor(string colorHex)
    {
        if (string.IsNullOrWhiteSpace(colorHex) || !Regex.IsMatch(colorHex, @"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$"))
        {
            return Result.Failure(new Error("TierRow.InvalidColorHex", "ColorHex must be a valid hexadecimal color code."));
        }

        ColorHex = colorHex;
        return Result.Success();
    }

    internal Result UpdateRank(string rank)
    {
        if (string.IsNullOrWhiteSpace(rank))
        {
            return Result.Failure(new Error("TierRow.EmptyRank", "Rank cannot be null or empty."));
        }

        if (rank.Length > MaxRankLength)
        {
            return Result.Failure(new Error("TierRow.RankTooLong", $"Rank cannot exceed {MaxRankLength} characters."));
        }

        Rank = rank;
        return Result.Success();
    }
}
