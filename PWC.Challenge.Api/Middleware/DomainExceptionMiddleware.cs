using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PWC.Challenge.Domain.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PWC.Challenge.Api.Middleware;

public class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DomainExceptionMiddleware> _logger;

    public DomainExceptionMiddleware(RequestDelegate next, ILogger<DomainExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain Exception: {Message}", ex.Message);
            await HandleDomainExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            await HandleGenericExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        context.Response.ContentType = "application/problem+json";
        var statusCode = GetStatusCodeForException(exception);
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = exception.GetType().Name.Replace("Exception", ""), // Remove "Exception" suffix for cleaner title
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "InternalServerError",
            Detail = "An unexpected internal server error has occurred.",
            Instance = context.Request.Path
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static HttpStatusCode GetStatusCodeForException(DomainException exception)
    {
        return exception switch
        {
            InvalidCarArgumentException => HttpStatusCode.BadRequest,
            RentalCarMismatchException => HttpStatusCode.BadRequest,
            CarNotAvailableException => HttpStatusCode.Conflict,
            OverlappingRentalException => HttpStatusCode.Conflict,
            OverlappingServiceException => HttpStatusCode.Conflict,
            CarRentedCannotBeInMaintenanceException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };
    }
}