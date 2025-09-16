using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PWC.Challenge.Common.Exceptions.Handlers;

public class CustomExceptionHandler(
    ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        var exceptionType = exception.GetType();
        var exceptionTypeName = exceptionType.Name;

        if (exceptionType.IsGenericType && exceptionTypeName.Contains('`'))
        {
            exceptionTypeName = exceptionTypeName.Substring(0, exceptionTypeName.IndexOf('`'));
        }

        var translationKey = "errors.unhandledException";
        var statusCode = StatusCodes.Status500InternalServerError;

        if (exception is IExceptionMetadata metadata)
        {
            translationKey = metadata.TranslationKey;
            statusCode = metadata.StatusCode;
        }
        else if (exception is ArgumentException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            translationKey = "errors.argumentException";
        }

        var problemDetails = new ProblemDetails
        {
            Title = exceptionTypeName,
            Detail = exception.Message,
            Status = statusCode,
            Instance = context.Request.Path,
        };
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        problemDetails.Extensions.Add("translationKey", translationKey);

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("validationErrors", validationException.Errors);
        }

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
