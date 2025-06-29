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

    public IEnumerable<TierRowEntity> GetRows(int listId)
    {
        return _context.TierImageContainers
            .OfType<TierRowEntity>()
            .Where(r => r.TierListId == listId)
            .Include(r => r.Images)
            .AsNoTracking()
            .ToList();
    }

    public IQueryable<TierRowEntity> GetRowsQueryable(int listId)
    {
        return _context.TierImageContainers
            .OfType<TierRowEntity>()
            .Where(r => r.TierListId == listId)
            .Include(r => r.Images)
            .AsNoTracking();
    }

    public TierRowEntity? GetRowById(int listId, int rowId)
    {
        return _context.TierImageContainers
            .OfType<TierRowEntity>()
            .AsNoTracking()
            .FirstOrDefault(r => r.Id == rowId && r.TierListId == listId);
    }

    public TierBackupRowEntity? GetBackupRow(int listId)
    {
       return _context.TierImageContainers
            .OfType<TierBackupRowEntity>()
            .AsNoTracking()
            .FirstOrDefault(r => r.TierListId == listId);
    }

    public IEnumerable<TierImageEntity> GetImages(int listId, int rowId)
    {
        return _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking()
            .ToList();
    }

    public IQueryable<TierImageEntity> GetImagesQueryable(int listId, int rowId)
    {
        return _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking();
    }

    public TierImageEntity? GetImageById(int listId, int rowId, int imageId)
    {
        return _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking()
            .FirstOrDefault(i => i.Id == imageId);
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
