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

    required public int FromContainerId { get; init; }

    required public int ToContainerId { get; init; }

    required public int Order { get; init; }
}