namespace TierList.Application.Commands.TierList;

public record CreateTierListCommand
{
    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    required public string Title { get; init; }
}
