using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Domain.Abstraction
{
    public interface IUnitOfWork
    {
        void CreateTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void SaveChanges();
    }
}
