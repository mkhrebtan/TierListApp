using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Commands;

public record UpdateTierListCommand
{
    public int Id { get; init; }

    public string? Title { get; init; }
}
