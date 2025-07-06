using System.Text.Json.Serialization;

namespace TierList.Application.Commands.TierList;

public record DeleteTierListCommand
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
