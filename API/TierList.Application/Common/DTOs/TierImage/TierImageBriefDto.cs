namespace TierList.Application.Common.DTOs.TierImage;

public class TierImageBriefDto
{
    public Guid StorageKey { get; init; }

    required public string Url { get; init; }
}
