using TierList.Application.Common.Enums;

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
    /// Generates an appropriate HTTP response based on the specified error type and message.
    /// </summary>
    /// <remarks>This method is typically used to standardize error handling in HTTP APIs by mapping
    /// application-specific error types to appropriate HTTP status codes and messages.</remarks>
    /// <param name="error">The type of error that occurred. Determines the HTTP status code of the response.</param>
    /// <param name="errorMessage">A descriptive message providing details about the error. Included in the response body.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response. The response will have a status code of: <list
    /// type="bullet"> <item><description><see cref="System.Net.HttpStatusCode.NotFound"/> if <paramref name="error"/>
    /// is <see cref="ErrorType.NotFound"/>.</description></item> <item><description><see
    /// cref="System.Net.HttpStatusCode.BadRequest"/> if <paramref name="error"/> is <see
    /// cref="ErrorType.ValidationError"/>.</description></item> <item><description><see
    /// cref="System.Net.HttpStatusCode.InternalServerError"/> if <paramref name="error"/> is <see
    /// cref="ErrorType.SaveDataError"/> or any other unrecognized value.</description></item> </list></returns>
    internal static IResult HandleError(ErrorType error, string errorMessage)
    {
        IResult errorResult = error switch
        {
            ErrorType.NotFound => TypedResults.NotFound(errorMessage),
            ErrorType.ValidationError => TypedResults.BadRequest(errorMessage),
            ErrorType.SaveDataError => TypedResults.InternalServerError(errorMessage),
            _ => TypedResults.InternalServerError("An unexpected error occurred while retrieving the upload URL.")
        };
        return errorResult;
    }
}
