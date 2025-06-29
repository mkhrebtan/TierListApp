namespace TierList.Domain.Entities;

public class TierRowEntity : TierImageContainer
{
    required public string Rank { get; set; } = string.Empty;

    required public string ColorHex { get; set; } = "#FFFFFF";

    public int Order { get; set; }
}
