using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

namespace TierList.Application.Queries.GetListData;

internal sealed class GetTierListDataQueryHandler : IQueryHandler<GetTierListDataQuery, TierListDataDto>
{
    private readonly ITierListRepository _tierListRepository;

    public GetTierListDataQueryHandler(ITierListRepository tierListRepository)
    {
        _tierListRepository = tierListRepository;
    }

    public async Task<Result<TierListDataDto>> Handle(GetTierListDataQuery query)
    {
        TierListEntity? tierList = await _tierListRepository.GetTierListWithDataAsync(query.Id);
        if (tierList is null)
        {
            return Result<TierListDataDto>.Failure(
                new Error("NotFound", $"List with ID {query.Id} not found."));
        }
        else if (tierList.UserId != query.UserId)
        {
            return Result<TierListDataDto>.Failure(
                new Error("Validation", $"List with ID {query.Id} does not belong to user with ID {query.UserId}."));
        }

        TierBackupRowEntity listBackupRowEntity = tierList.GetTierBackupRow();

        TierListDataDto tierListData = new()
        {
            Id = tierList.Id,
            Title = tierList.Title,
            Rows = GetTierRowDtos(tierList),
            BackupRow = GetTierBackupRowDto(listBackupRowEntity),
        };

        return Result<TierListDataDto>.Success(tierListData);
    }

    private static IReadOnlyCollection<TierRowDto> GetTierRowDtos(TierListEntity listEntity)
    {
        IEnumerable<TierRowEntity> rowEntities = listEntity.GetTierRows();
        List<TierRowDto> tierRowDtos = new();

        foreach (var rowEntity in rowEntities)
        {
            List<TierImageDto> rowImages = new();
            var rowImageEntities = rowEntity.Images;
            if (rowImageEntities.Count != 0)
            {
                foreach (var image in rowImageEntities)
                {
                    rowImages.Add(new TierImageDto
                    {
                        Id = image.Id,
                        StorageKey = image.StorageKey,
                        Url = image.Url,
                        Note = image.Note,
                        ContainerId = image.ContainerId,
                        Order = image.Order.Value,
                    });
                }

                rowImages = rowImages.OrderBy(i => i.Order).ToList();
            }

            tierRowDtos.Add(new TierRowDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order.Value,
                Images = rowImages.AsReadOnly(),
            });
        }

        return tierRowDtos.OrderBy(r => r.Order).ToList().AsReadOnly();
    }

    private static TierBackupRowDto GetTierBackupRowDto(TierBackupRowEntity backupRowEntity)
    {
        List<TierImageDto> backupImages = new();
        var backupRowImages = backupRowEntity.Images;
        if (backupRowImages.Count != 0)
        {
            foreach (var image in backupRowImages)
            {
                backupImages.Add(new TierImageDto
                {
                    Id = image.Id,
                    StorageKey = image.StorageKey,
                    Url = image.Url,
                    Note = image.Note,
                    ContainerId = image.ContainerId,
                    Order = image.Order.Value,
                });
            }

            backupImages = backupImages.OrderBy(i => i.Order).ToList();
        }

        return new TierBackupRowDto
        {
            Id = backupRowEntity.Id,
            Images = backupImages.AsReadOnly(),
        };
    }
}