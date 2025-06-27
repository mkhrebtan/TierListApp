using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Queries.DTOs;

public class TierListBriefDTO
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public DateTime Created { get; init; }
    public DateTime LastModified { get; init; }
}
