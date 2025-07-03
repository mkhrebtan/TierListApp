using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierRow;

public record UpdateTierRowOrderCommand : IUpdateRowCommand
{
    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public int Order { get; set; }

    public void UpdateRow(TierRowEntity rowEntity)
    {
        if (Order < 0)
        {
            throw new ArgumentException("Order cannot be negative.");
        }

        rowEntity.Order = Order;
    }
}
