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
    Task<IEnumerable<TierListEntity>> GetAllAsync();

    IQueryable<TierListEntity> GetAllQueryable();

    void AddRow(TierRowEntity rowEntity);

    void DeleteRow(TierRowEntity rowEntity);

    void UpdateRow(TierRowEntity rowEntity);

    Task<IEnumerable<TierRowEntity>> GetRowsAsync(int listId);

    IQueryable<TierRowEntity> GetRowsQueryable(int listId);

    Task<TierRowEntity?> GetRowByIdAsync(int listId, int rowId);

    Task<TierBackupRowEntity?> GetBackupRowAsync(int listId);

    Task<IEnumerable<TierImageEntity>> GetImagesAsync(int listId, int rowId);

    IQueryable<TierImageEntity> GetImagesQueryable(int listId, int rowId);

    Task<TierImageEntity?> GetImageByIdAsync(int listId, int rowId, int imageId);

    void AddImage(TierImageEntity imageEntity);

    void DeleteImage(TierImageEntity imageEntity);

    void UpdateImage(TierImageEntity imageEntity);
}
