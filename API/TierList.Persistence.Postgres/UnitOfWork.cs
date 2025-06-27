using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TierList.Domain.Abstraction;

namespace TierList.Persistence.Postgres;

/// <summary>
/// Provides a Unit of Work implementation for managing database transactions and operations
/// using Entity Framework Core with PostgreSQL as the data store.
/// </summary>
internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TierListDbContext _context;

    private bool _disposed = false;

    private IDbContextTransaction? _contextTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context to use for operations.</param>
    public UnitOfWork(TierListDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new database transaction for the current unit of work.
    /// </summary>
    public void CreateTransaction()
    {
        _contextTransaction = _context.Database.BeginTransaction();
    }

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction has been created.</exception>
    public void CommitTransaction()
    {
        HandleTransactionCreation();
        _contextTransaction?.Commit();
    }

    /// <summary>
    /// Rolls back the current database transaction and disposes of it.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction has been created.</exception>
    public void RollbackTransaction()
    {
        HandleTransactionCreation();
        _contextTransaction?.Rollback();
        _contextTransaction?.Dispose();
    }

    /// <summary>
    /// Saves all changes made in the current context to the database.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a database update error occurs or an unexpected error happens during save operation.
    /// </exception>
    public void SaveChanges()
    {
        string errorMessage;
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            errorMessage = "An error occurred while saving changes to the database.";
            throw new InvalidOperationException(errorMessage, ex);
        }
        catch (Exception ex)
        {
            errorMessage = "An unexpected error occurred while saving changes.";
            throw new InvalidOperationException(errorMessage, ex);
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="UnitOfWork"/> instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="UnitOfWork"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Validates that a transaction has been created before performing transaction operations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction has been created.</exception>
    private void HandleTransactionCreation()
    {
        if (_contextTransaction == null)
        {
            throw new InvalidOperationException("No transaction has been created.");
        }
    }
}
