using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierRow.UpdateOrder;

public sealed record UpdateTierRowOrderCommand : ICommand<TierRowBriefDto>
{
    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public int Order { get; set; }
}
