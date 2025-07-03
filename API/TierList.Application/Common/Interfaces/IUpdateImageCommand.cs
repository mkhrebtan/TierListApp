using TierList.Domain.Entities;

namespace TierList.Application.Common.Interfaces;

public interface IUpdateImageCommand
{
    int Id { get; init; }

    int ListId { get; init; }

    int ContainerId { get; init; }

    void Update(TierImageEntity imageEntity);
}
