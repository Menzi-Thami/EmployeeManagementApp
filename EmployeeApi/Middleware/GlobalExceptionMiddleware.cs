using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EmployeeApi.Middleware
{
    /// <summary>
    /// Translates typed application exceptions into HTTP status codes:
    /// NotFoundException -> 404, ArgumentException -> 400, everything else -> 500.
    /// Exceptions are never swallowed; they are logged and mapped to a response.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);
                await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                status = (int)statusCode,
                error = message
            });

            return context.Response.WriteAsync(payload);
        }
    }
}
