using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Queries.DTOs;

public class TierImageDTO
{
    public int Id { get; init; }

    required public string Url { get; init; }

    required public string Note { get; init; }

    public int ContainerId { get; init; }

    public int Order { get; init; }
}
