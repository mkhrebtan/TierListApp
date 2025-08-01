﻿using TierList.Application.Common.DTOs.TierImage;

namespace TierList.Application.Common.DTOs.TierRow;

/// <summary>
/// Represents a data transfer object (DTO) for a tier backup row, containing an identifier and associated images.
/// </summary>
public class TierBackupRowDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the collection of images associated with the tier.
    /// </summary>
    public IReadOnlyCollection<TierImageDto> Images { get; init; } = new List<TierImageDto>();
}
