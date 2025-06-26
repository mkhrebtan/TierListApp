using TierList.Domain.Abstraction;
using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

public interface ITierImageRepository : IRepository<TierImageEntity>
{
    IEnumerable<TierImageEntity> GetRowImages(int rowId);
}
