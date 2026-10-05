using EmployeeManagementApp.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.ErrorHandling
{
    /// <summary>
    /// Maps exceptions on /api routes to RFC 9457 ProblemDetails:
    /// NotFoundException -> 404, ValidationException -> 400, anything else -> 500 with a
    /// generic title (no exception text). MVC pages are left to UseExceptionHandler's
    /// re-execution of /Home/Error, so a browser never receives a raw JSON body.
    /// </summary>
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // UseExceptionHandler("/Home/Error") has already rewritten Request.Path; use the original.
            var path = new PathString(httpContext.Features.Get<IExceptionHandlerPathFeature>()?.Path ?? httpContext.Request.Path);
            if (!path.StartsWithSegments("/api"))
            {
                return false;
            }

            ProblemDetails problem = exception switch
            {
                NotFoundException notFound => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource not found.",
                    Detail = notFound.Message,
                    Extensions = { ["code"] = "not_found" }
                },
                ValidationException validation => new ValidationProblemDetails(
                    validation.Errors.ToDictionary(e => e.Key, e => e.Value))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                    Extensions = { ["code"] = "validation_failed" }
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred.",
                    Extensions = { ["code"] = "unexpected_error" }
                }
            };

            if (problem.Status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception processing {Path}", path);
            }

            httpContext.Response.StatusCode = problem.Status!.Value;
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception
            });
        }
    }
}
