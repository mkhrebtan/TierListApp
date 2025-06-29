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
/// Provides a unit of work implementation for managing database transactions and saving changes within a single
/// context. This class ensures that all operations are performed atomically and supports transaction management for
/// consistent data operations.
/// </summary>
/// <remarks>The <see cref="UnitOfWork"/> class is designed to encapsulate database operations within a single
/// transactional context. It provides methods to create, commit, and roll back transactions, as well as to save changes
/// to the database. This implementation ensures that resources are properly disposed of when the unit of work is no
/// longer needed.  This class is not thread-safe and should be used within a single thread or request
/// context.</remarks>
internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TierListDbContext _context;

    private bool _disposed = false;

    private IDbContextTransaction? _contextTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The <see cref="TierListDbContext"/> instance used to manage database operations. Cannot be null.</param>
    public UnitOfWork(TierListDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <remarks>This method initializes a new transaction for the current database context.  Ensure that the
    /// transaction is committed or rolled back appropriately to avoid  leaving the database in an inconsistent
    /// state.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CreateTransactionAsync()
    {
        _contextTransaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current database transaction asynchronously.
    /// </summary>
    /// <remarks>This method finalizes the transaction, persisting all changes made during the transaction
    /// scope. Ensure that a transaction has been created before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TransactionNotCreatedException">Thrown if no transaction has been created prior to calling this method.</exception>
    public async Task CommitTransactionAsync()
    {
        await _contextTransaction!.CommitAsync();
    }

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <remarks>This method reverts all changes made during the current transaction and disposes of the
    /// transaction object. Ensure that a transaction has been created before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TransactionNotCreatedException">Thrown if no transaction has been created prior to calling this method.</exception>
    public async Task RollbackTransactionAsync()
    {
        await _contextTransaction!.RollbackAsync();
        await Task.Run(() =>
        {
            _contextTransaction.Dispose();
        });
    }

    /// <summary>
    /// Asynchronously saves all changes made in the current context to the database.
    /// </summary>
    /// <remarks>This method commits all tracked changes to the underlying database. If an error occurs during
    /// the save operation, a <see cref="SaveChangesException"/> is thrown with a descriptive error message.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="SaveChangesException">Thrown when an error occurs while saving changes to the database.</exception>
    public async Task SaveChangesAsync()
    {
        string errorMessage;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            errorMessage = "An unexpected error occurred while saving changes to the database.";
            throw new InvalidOperationException(errorMessage);
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
}
