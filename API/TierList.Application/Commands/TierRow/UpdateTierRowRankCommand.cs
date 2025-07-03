using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierRow;

public record UpdateTierRowRankCommand : IUpdateRowCommand
{
    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public string Rank { get; set; }

    public void UpdateRow(TierRowEntity rowEntity)
    {
        if (string.IsNullOrEmpty(Rank))
        {
            throw new ArgumentException("Rank cannot be null or empty.");
        }

        rowEntity.Rank = Rank;
    }
}
