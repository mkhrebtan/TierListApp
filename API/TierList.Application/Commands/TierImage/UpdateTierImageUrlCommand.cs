using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierImage;

public partial class UpdateTierImageUrlCommand : IUpdateImageCommand
{
    private static Regex _urlRegex = CreateUrlRegex();

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
        else if (!_urlRegex.IsMatch(Url))
        {
            throw new ArgumentException("Invalid URL format.");
        }

        imageEntity.Url = Url;
    }

    [GeneratedRegex(@"/^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$/")]
    private static partial Regex CreateUrlRegex();
}