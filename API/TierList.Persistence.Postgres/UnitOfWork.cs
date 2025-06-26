using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Abstraction;

namespace TierList.Persistence.Postgres;

internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool _disposed = false;
    private string _errorMessage = string.Empty;

    private readonly TierListDbContext _context;

    private IDbContextTransaction? _contextTransaction;

    public UnitOfWork(TierListDbContext context)
    {
        _context = context;
    }

    public void CreateTransaction()
    {
        _contextTransaction = _context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        HandleTransactionCreation();
        _contextTransaction?.Commit();
    }

    public void RollbackTransaction()
    {
        HandleTransactionCreation();
        _contextTransaction?.Rollback();
        _contextTransaction?.Dispose();
    }

    public void SaveChanges()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            _errorMessage = "An error occurred while saving changes to the database.";
            throw new InvalidOperationException(_errorMessage, ex);
        }
        catch (Exception ex)
        {
            _errorMessage = "An unexpected error occurred while saving changes.";
            throw new InvalidOperationException(_errorMessage, ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _context.Dispose();
        _disposed = true;
    }

    private void HandleTransactionCreation()
    {
        if (_contextTransaction == null)
        {
            _errorMessage = "No transaction has been created.";
            throw new InvalidOperationException(_errorMessage);
        }
    }
}
