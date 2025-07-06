using System.Text.Json.Serialization;

namespace TierList.Application.Commands.TierList;

public record CreateTierListCommand
{
    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    required public string Title { get; init; }

    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    [JsonIgnore]
    public int UserId { get; set; }
}
