namespace TierList.Domain.Abstraction;

/// <summary>
/// Defines a contract for managing database transactions and persisting changes to the underlying data store.
/// </summary>
/// <remarks>This interface provides methods for creating, committing, and rolling back transactions, as well as
/// saving changes to the data store. It is typically used to ensure atomicity and consistency in operations that
/// involve multiple changes to the database.</remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously creates a new transaction.
    /// </summary>
    /// <remarks>This method initiates the creation of a transaction. The specific details of the transaction
    /// are determined by the implementation. Ensure that any required preconditions are met before calling this
    /// method.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateTransactionAsync();

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <remarks>This method finalizes the transaction, making all changes within the transaction permanent.
    /// Ensure that all necessary operations within the transaction are complete before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Asynchronously rolls back the current transaction, undoing all changes made during the transaction.
    /// </summary>
    /// <remarks>This method should be called to cancel a transaction and revert any changes made since the
    /// transaction began. Ensure that a transaction is active before calling this method to avoid unexpected
    /// behavior.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Asynchronously saves all changes made in the current context to the underlying database.
    /// </summary>
    /// <remarks>This method commits all tracked changes, such as additions, updates, and deletions, to the
    /// database. It is recommended to call this method after making modifications to ensure data consistency.</remarks>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync();
}
