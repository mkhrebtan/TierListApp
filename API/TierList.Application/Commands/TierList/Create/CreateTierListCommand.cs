using System.Text.Json.Serialization;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierList.Create;

public sealed record CreateTierListCommand : ICommand<TierListBriefDto>
{
    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    required public string Title { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [JsonIgnore]
    public int UserId { get; set; }
}