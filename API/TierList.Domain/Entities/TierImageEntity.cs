using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

public class TierImageEntity : IEntity
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public int Order { get; set; }
    public string Note { get; set; } = String.Empty;
    public int ContainerId { get; set; }

    // Navigation properties
    public TierImageContainer? Container { get; set; }
}