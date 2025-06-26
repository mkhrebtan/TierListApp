using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

public interface ITierImageContainerRepository : IRepository<TierImageContainer>
{
    IEnumerable<TierRowEntity> GetListRows(int listId);
    TierBackupRowEntity GetListBackupRow(int listId);
}
