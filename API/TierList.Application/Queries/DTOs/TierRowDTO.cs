using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Queries.DTOs;

public class TierRowDTO
{
    public int Id { get; init; }

    required public string Rank { get; init; }

    required public string ColorHex { get; init; }

    public int Order { get; init; }

    public IReadOnlyCollection<TierImageDTO> Images { get; init; } = new List<TierImageDTO>();
}
