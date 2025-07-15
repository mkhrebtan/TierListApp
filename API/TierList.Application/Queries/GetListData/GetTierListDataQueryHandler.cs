using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

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
        TierListEntity? tierList = await _tierListRepository.GetByIdAsync(query.Id);
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

        TierBackupRowEntity? listBackupRowEntity = await _tierListRepository.GetBackupRowAsync(tierList.Id);
        if (listBackupRowEntity is null)
        {
            return Result<TierListDataDto>.Failure(
                new Error("NotFound", $"Backup row for list with ID {query.Id} not found."));
        }

        IReadOnlyCollection<TierRowDto> listRows = await GetTierRowDtosAsync(tierList);
        TierBackupRowDto listBackupRowDto = await GetTierBackupRowDto(listBackupRowEntity);

        TierListDataDto tierListData = new()
        {
            Id = tierList.Id,
            Title = tierList.Title,
            Rows = listRows,
            BackupRow = listBackupRowDto,
        };

        return Result<TierListDataDto>.Success(tierListData);
    }

    private async Task<IReadOnlyCollection<TierRowDto>> GetTierRowDtosAsync(TierListEntity listEntity)
    {
        IEnumerable<TierRowEntity> rowEntities = await _tierListRepository.GetRowsWithImagesAsync(listEntity.Id);
        List<TierRowDto> tierRowDtos = new();

        foreach (var rowEntity in rowEntities)
        {
            List<TierImageDto> rowImages = new();
            IEnumerable<TierImageEntity> rowImagesEntities = await _tierListRepository.GetImagesAsync(rowEntity.Id);
            if (rowImagesEntities.Any())
            {
                foreach (var image in rowEntity.Images)
                {
                    rowImages.Add(new TierImageDto
                    {
                        Id = image.Id,
                        StorageKey = image.StorageKey,
                        Url = image.Url,
                        Note = image.Note,
                        ContainerId = image.ContainerId,
                        Order = image.Order,
                    });
                }

                rowImages = rowImages.OrderBy(i => i.Order).ToList();
            }

            tierRowDtos.Add(new TierRowDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order,
                Images = rowImages.AsReadOnly(),
            });
        }

        return tierRowDtos.OrderBy(r => r.Order).ToList().AsReadOnly();
    }

    private async Task<TierBackupRowDto> GetTierBackupRowDto(TierBackupRowEntity backupRowEntity)
    {
        List<TierImageDto> backupImages = new();
        IEnumerable<TierImageEntity> backupRowImages = await _tierListRepository.GetImagesAsync(backupRowEntity.Id);
        if (backupRowImages.Any())
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
                    Order = image.Order,
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