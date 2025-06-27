using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos;

public class TierListRepository : GenericRepository<TierListEntity>, ITierListRepository
{
    public TierListRepository(TierListDbContext context)
        : base(context)
    {
    }

    public IEnumerable<TierListEntity> GetAll()
    {
        return _context.TierLists
            .AsNoTracking()
            .ToList();
    }

    public IQueryable<TierListEntity> GetAllQueryable()
    {
        return _context.TierLists
            .AsNoTracking();
    }

    public void AddRow(TierRowEntity rowEntity)
    {
        _context.TierImageContainers.Add(rowEntity);
    }

    public void DeleteRow(TierRowEntity rowEntity)
    {
       _context.TierImageContainers.Remove(rowEntity);
    }

    public void UpdateRow(TierRowEntity rowEntity)
    {
        _context.TierImageContainers.Update(rowEntity);
    }

    public TierListEntity? GetByIdWithAllData(int listId)
    {
        return _context.TierLists
            .Include(tl => tl.Containers)
                .ThenInclude(c => c.Images)
            .AsNoTracking()
            .SingleOrDefault(tl => tl.Id == listId);
    }

    public TierRowEntity? GetRowById(int listId, int rowId)
    {
        return _context.TierImageContainers
            .OfType<TierRowEntity>()
            .AsNoTracking()
            .SingleOrDefault(r => r.Id == rowId && r.TierListId == listId);
    }

    public TierBackupRowEntity? GetBackupRow(int listId)
    {
       return _context.TierImageContainers
            .OfType<TierBackupRowEntity>()
            .AsNoTracking()
            .SingleOrDefault(r => r.TierListId == listId);
    }

    public TierImageEntity? GetImageById(int listId, int rowId, int imageId)
    {
        return _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking()
            .SingleOrDefault(i => i.Id == imageId);
    }

    public void AddImage(TierImageEntity imageEntity)
    {
        _context.TierImages.Add(imageEntity);
    }

    public void DeleteImage(TierImageEntity imageEntity)
    {
       _context.TierImages.Remove(imageEntity);
    }

    public void UpdateImage(TierImageEntity imageEntity)
    {
        _context.TierImages.Update(imageEntity);
    }
}
