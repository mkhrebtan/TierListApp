using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Commands.TierImage;

public record MoveTierImageCommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public int Order { get; init; }

    required public bool IsMoveToOtherContainer { get; init; }
}
