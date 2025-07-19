using TierList.Domain.Shared;

namespace TierListAPI.Helpers;

/// <summary>
/// Provides methods for handling errors and generating appropriate HTTP responses based on error types.
/// </summary>
/// <remarks>This class is designed to map specific error types to corresponding HTTP responses,  ensuring
/// consistent error handling across the application. It supports common error scenarios  such as "Not Found",
/// "Validation Error", and "Internal Server Error".</remarks>
internal static class ErrorHandler
{
    /// <summary>
    /// Generates an appropriate HTTP response based on the specified error code.
    /// </summary>
    /// <param name="error">The error object containing the code and message to be processed.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response. Returns a 404 Not Found response for "NotFound" errors,
    /// a 400 Bad Request response for "Validation" errors, and a 500 Internal Server Error response for
    /// "SaveDataError", "UnexpectedError", or any unrecognized error codes.</returns>
    internal static IResult HandleError(Error error)
    {
        IResult errorResult = error.Code switch
        {
            "NotFound" => TypedResults.NotFound(error.Message),
            "Validation" => TypedResults.BadRequest(error.Message),
            "SaveDataError" => TypedResults.InternalServerError(error.Message),
            "UnexpectedError" => TypedResults.InternalServerError(error.Message),
            _ => TypedResults.InternalServerError("An unexpected error occurred.")
        };
        return errorResult;
    }
}
