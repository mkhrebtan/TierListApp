using System.Text.Json.Serialization;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierList.Delete;

public sealed record DeleteTierListCommand : ICommand
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    required public int Id { get; init; }

    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    [JsonIgnore]
    public int UserId { get; set; }
}