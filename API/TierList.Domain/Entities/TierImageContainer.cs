using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

public abstract class TierImageContainer : IEntity
{
    public int Id { get; set; }
    public int TierListId { get; set; }

    // Navigation properties
    public TierListEntity? TierList { get; set; }
    public List<TierImageEntity> Images { get; set; } = new List<TierImageEntity>();
}
