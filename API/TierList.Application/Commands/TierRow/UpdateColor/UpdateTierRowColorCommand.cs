using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierRow;

public sealed record UpdateTierRowColorCommand : ICommand<TierRowBriefDto>
{
    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public string ColorHex { get; set; }
}