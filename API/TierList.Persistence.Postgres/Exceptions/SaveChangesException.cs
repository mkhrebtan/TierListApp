namespace TierList.Persistence.Postgres.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an error occurs while saving changes to the data store.
/// </summary>
/// <remarks>This exception is typically used to indicate a failure during a database save operation, such as when
/// a conflict or validation error occurs. It provides an optional error message to describe the specific issue
/// encountered.</remarks>
public class SaveChangesException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveChangesException"/> class with a default error message.
    /// </summary>
    public SaveChangesException()
        : base("An error occurred while saving changes.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveChangesException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SaveChangesException(string message)
        : base(message)
    {
    }
}