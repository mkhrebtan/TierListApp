using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

public interface ITierListRepository : IRepository<TierListEntity>
{
    IEnumerable<TierListEntity> GetAll();

    IQueryable<TierListEntity> GetAllQueryable();

    void AddRow(TierRowEntity rowEntity);

    void DeleteRow(TierRowEntity rowEntity);

    void UpdateRow(TierRowEntity rowEntity);

    IEnumerable<TierRowEntity> GetRows(int listId);

    IQueryable<TierRowEntity> GetRowsQueryable(int listId);

    TierRowEntity? GetRowById(int listId, int rowId);

    TierBackupRowEntity? GetBackupRow(int listId);

    IEnumerable<TierImageEntity> GetImages(int listId, int rowId);

    IQueryable<TierImageEntity> GetImagesQueryable(int listId, int rowId);

    TierImageEntity? GetImageById(int listId, int rowId, int imageId);

    void AddImage(TierImageEntity imageEntity);

    void DeleteImage(TierImageEntity imageEntity);

    void UpdateImage(TierImageEntity imageEntity);
}
