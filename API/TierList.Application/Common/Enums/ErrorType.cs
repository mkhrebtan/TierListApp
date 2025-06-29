namespace TierList.Application.Common.Enums;

/// <summary>
/// Represents the types of errors that can occur during application execution.
/// </summary>
/// <remarks>This enumeration is used to categorize errors into specific types, allowing for more precise error
/// handling and reporting. Each value represents a distinct category of error: <list type="bullet"> <item>
/// <description><see cref="NotFound"/> indicates that a requested resource could not be found.</description> </item>
/// <item> <description><see cref="ValidationError"/> indicates that input data failed validation checks.</description>
/// </item> <item> <description><see cref="SaveDataError"/> indicates an error occurred while attempting to save
/// data.</description> </item> <item> <description><see cref="UnexpectedError"/> indicates an error that does not fall
/// into any specific category.</description> </item> </list></remarks>
public enum ErrorType
{
    /// <summary>
    /// Represents a result indicating that the requested resource was not found.
    /// </summary>
    /// <remarks>This value is typically used to signify that an operation failed because the target resource
    /// does not exist or could not be located. It is often used in scenarios such as HTTP responses or error handling
    /// in APIs.</remarks>
    NotFound,

    /// <summary>
    /// Represents an error that occurs during validation.
    /// </summary>
    /// <remarks>This class is typically used to encapsulate details about a validation failure,  such as the
    /// error message and the field or property that caused the error.</remarks>
    ValidationError,

    /// <summary>
    /// Represents an error that occurs during a data save operation.
    /// </summary>
    /// <remarks>This enumeration is used to categorize the types of errors that can occur when attempting to
    /// save data. It can be used to identify the specific issue and take appropriate action based on the error
    /// type.</remarks>
    SaveDataError,

    /// <summary>
    /// Represents an error that occurs unexpectedly during application execution.
    /// </summary>
    /// <remarks>This class is typically used to encapsulate details about an error that was not anticipated
    /// or explicitly handled by the application. It can be used to provide additional context or information about the
    /// error for logging or debugging purposes.</remarks>
    UnexpectedError,
}
