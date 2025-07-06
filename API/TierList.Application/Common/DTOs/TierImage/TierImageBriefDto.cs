using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.DTOs.TierImage;

public class TierImageBriefDto : ITierImageDto
{
    public Guid StorageKey { get; init; }

    required public string Url { get; init; }
}
