using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Abstraction;

namespace TierList.Persistence.Postgres.Repos;

public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly TierListDbContext _context;

    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(TierListDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>() ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not registered in the DbContext.");
    }

    public TEntity? GetById(int id)
    {
        return _dbSet.Find(id);
    }
    public void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }
    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}
