using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Commands;

public record CreateTierListCommand
{
    public required string Title { get; init; }
}
