namespace TierList.Domain.Entities;

public class TierRowEntity : TierImageContainer
{
    public string? Rank { get; set; }
    public string? ColorHex { get; set; }
    public int? Order { get; set; }
}
