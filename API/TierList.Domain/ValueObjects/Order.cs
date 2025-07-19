using TierList.Domain.Abstraction;
using TierList.Domain.Shared;

namespace TierList.Domain.ValueObjects;

public class Order : ValueObject
{
    public const int MinValue = 1;

    private Order(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static bool operator ==(Order? left, Order? right)
    {
        return left is not null && left.Equals(right);
    }

    public static bool operator !=(Order? left, Order? right)
    {
        return left is not null && !left.Equals(right);
    }

    public static bool operator <(Order left, Order right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(Order left, Order right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <=(Order left, Order right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator >=(Order left, Order right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator ==(Order left, int right)
    {
        return left is not null && left.Value == right;
    }

    public static bool operator !=(Order left, int right)
    {
        return left is not null && left.Value != right;
    }

    public static bool operator <(Order left, int right)
    {
        return left is not null && left.Value < right;
    }

    public static bool operator >(Order left, int right)
    {
        return left is not null && left.Value > right;
    }

    public static bool operator <=(Order left, int right)
    {
        return left is not null && left.Value <= right;
    }

    public static bool operator >=(Order left, int right)
    {
        return left is not null && left.Value >= right;
    }

    public static Result<Order> Create(int value)
    {
        if (value < MinValue)
        {
            return Result<Order>.Failure(new Error("Order.InvalidValue", $"Order value must be greater than or equal to {MinValue}."));
        }

        return Result<Order>.Success(new Order(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    internal Order Increment()
    {
        return Create(Value + 1).Value;
    }

    internal Result<Order> Decrement()
    {
        if (Value <= MinValue)
        {
            return Result<Order>.Failure(new Error("Order.InvalidDecrement", $"Order value cannot be less than {MinValue}."));
        }

        return Create(Value - 1);
    }
}
