using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.DTOs;

/// <summary>
/// Represents the data transfer object (DTO) for a tier list, including its identifier, title, tiers, and backup row
/// information.
/// </summary>
/// <remarks>This DTO is used to encapsulate the data structure of a tier list, which includes a unique
/// identifier, a title,  a collection of tier rows, and a backup row. It is designed to be immutable and is typically
/// used for transferring  tier list data between application layers or services.</remarks>
public class TierListDataDto : ITierListDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    required public string Title { get; init; }

    /// <summary>
    /// Gets the collection of rows representing the tiers.
    /// </summary>
    public IReadOnlyCollection<TierRowDto> Rows { get; init; } = new List<TierRowDto>();

    /// <summary>
    /// Gets the backup row data associated with the tier.
    /// </summary>
    public TierBackupRowDto BackupRow { get; init; }
}
