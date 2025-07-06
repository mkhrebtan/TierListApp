using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierImage;

public record UpdateTierImageUrlCommand : IUpdateImageCommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public string Url { get; init; }

    public void Update(TierImageEntity imageEntity)
    {
        if (string.IsNullOrEmpty(Url))
        {
            throw new ArgumentException("Url cannot be null or empty.");
        }

        imageEntity.Url = Url;
    }
}