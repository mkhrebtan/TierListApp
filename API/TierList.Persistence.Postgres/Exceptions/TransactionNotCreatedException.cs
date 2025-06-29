using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Persistence.Postgres.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a transaction has not been created prior to an operation that requires
/// it.
/// </summary>
/// <remarks>This exception is typically thrown when an attempt is made to commit or use a transaction without
/// first creating it.</remarks>
public class TransactionNotCreatedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionNotCreatedException"/> class with a default error message.
    /// </summary>
    public TransactionNotCreatedException()
        : base("Transaction was not created. Please ensure that CreateTransactionAsync is called before committing the transaction.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionNotCreatedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TransactionNotCreatedException(string message)
        : base(message)
    {
    }
}