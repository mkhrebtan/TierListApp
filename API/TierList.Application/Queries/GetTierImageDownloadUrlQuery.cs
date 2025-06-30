using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Queries;

public record GetTierImageDownloadUrlQuery
{
    public Guid StorageKey { get; init; }
}
