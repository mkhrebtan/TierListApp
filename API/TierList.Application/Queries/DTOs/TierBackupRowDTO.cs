using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Queries.DTOs;

public class TierBackupRowDTO
{
    public int Id { get; init; }

    public IReadOnlyCollection<TierImageDTO> Images { get; init; } = new List<TierImageDTO>();
}
