using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Queries.DTOs;

public class TierListDataDTO : ITierListDTO
{
    public int Id { get; init; }

    required public string Title { get; init; }

    public IReadOnlyCollection<TierRowDTO> Rows { get; init; } = new List<TierRowDTO>();

    public TierBackupRowDTO BackupRow { get; init; }
}
