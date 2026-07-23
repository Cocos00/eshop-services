using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(
                exception,
                "Error Message: {ExceptionMessage}, Time of occurrence: {Time}",
                exception.Message,
                DateTime.UtcNow);

            var details = exception switch
            {
                InternalServerException => (
                    Detail: exception.Message,
                    Title: exception.GetType().Name,
                    StatusCode: StatusCodes.Status500InternalServerError),
                ValidationException => (
                    Detail: exception.Message,
                    Title: exception.GetType().Name,
                    StatusCode: StatusCodes.Status400BadRequest),
                BadRequestException => (
                    Detail: exception.Message,
                    Title: exception.GetType().Name,
                    StatusCode: StatusCodes.Status400BadRequest),
                NotFoundException => (
                    Detail: exception.Message,
                    Title: exception.GetType().Name,
                    StatusCode: StatusCodes.Status404NotFound),
                _ => (
                    Detail: exception.Message,
                    Title: exception.GetType().Name,
                    StatusCode: StatusCodes.Status500InternalServerError)
            };

            context.Response.StatusCode = details.StatusCode;

            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path,
            };

            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            if (exception is ValidationException validationException)
            {
                problemDetails.Extensions.Add("validationErrors", validationException.Errors);
            }

            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
