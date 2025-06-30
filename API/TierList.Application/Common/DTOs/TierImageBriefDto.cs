using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.DTOs;

public class TierImageBriefDto : ITierImageDto
{
    public Guid StorageKey { get; init; }

    required public string Url { get; init; }
}
