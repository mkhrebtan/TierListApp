using System.ComponentModel.DataAnnotations.Schema;
using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

public class TierListEntity : IEntity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }

    // Navigation properties
    public List<TierImageContainer> Containers { get; set; } = new List<TierImageContainer>();
}
