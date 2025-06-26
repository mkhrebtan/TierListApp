using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos
{
    public class TierImageContainerRepository : ITierImageContainerRepository
    {
        private readonly TierListDbContext _context;

        public TierImageContainerRepository(TierListDbContext context)
        {
            _context = context;
        }

        public TierImageContainer? GetById(int id)
        {
            return _context.TierImageContainers.Find(id);
        }

        public IEnumerable<TierRowEntity> GetListRows(int listId)
        {
            return _context.TierImageContainers
                .OfType<TierRowEntity>()
                .Where(r => r.TierListId == listId)
                .AsNoTracking()
                .ToList();
        }

        public TierBackupRowEntity GetListBackupRow(int listId)
        {
            try
            {
                return _context.TierImageContainers
                    .OfType<TierBackupRowEntity>()
                    .AsNoTracking()
                    .Single(r => r.TierListId == listId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Expected one backup row for TierList {listId}, but found zero or multiple.", ex);
            }
        }

        public void Insert(TierImageContainer entity)
        {
            string containerType = entity is TierRowEntity ? "row" : "backup";

            try
            {
                _context.TierImageContainers.Add(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"An error occurred while inserting the {containerType} container entity.", ex);
            }
        }

        public void Update(TierImageContainer entity)
        {
            string containerType = entity is TierRowEntity ? "row" : "backup";

            try
            {
                _context.TierImageContainers.Update(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"An error occurred while updating the {containerType} container entity.", ex);
            }
        }

        public void Delete(TierImageContainer entity)
        {
            string containerType = entity is TierRowEntity ? "row" : "backup";

            try
            {
                _context.TierImageContainers.Remove(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the {containerType} container entity.", ex);
            }
        }
    }
}
