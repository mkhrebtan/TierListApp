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
    void AddRow(TierRowEntity rowEntity);
    void DeleteRow(TierRowEntity rowEntity);
    void UpdateRow(TierRowEntity rowEntity);
    TierListEntity? GetByIdWithAllData(int listId);
    TierRowEntity? GetRowById(int listId, int rowId);
    TierBackupRowEntity? GetBackupRow(int listId);
    TierImageEntity? GetImageById(int listId, int rowId, int imageId);
    void AddImage(TierImageEntity imageEntity);
    void DeleteImage(TierImageEntity imageEntity);
    void UpdateImage(TierImageEntity imageEntity);
}
