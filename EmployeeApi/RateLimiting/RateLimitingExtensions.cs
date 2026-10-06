using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeeApi.RateLimiting
{
    /// <summary>
    /// Per-client-IP limits (the app has no sign-in, so IP is the only partition key).
    /// Every routed request counts against <see cref="GlobalPermitsPerMinute"/>; the form post
    /// and the JSON API add a stricter named policy on top. Static files are served before
    /// routing and never reach the limiter; health endpoints opt out explicitly.
    /// Limits are per instance: scaled out, they multiply by the instance count. Behind a
    /// reverse proxy, configure ForwardedHeaders with the proxy's address first, or every
    /// client shares the proxy's IP (and never trust X-Forwarded-For from anyone else).
    /// </summary>
    public static class RateLimitingExtensions
    {
        public const string ApiPolicy = "api";
        public const string FormPostPolicy = "form-post";

        public const int GlobalPermitsPerMinute = 300;
        public const int ApiPermitsPerMinute = 60;
        public const int FormPostPermitsPerMinute = 10;

        public static IServiceCollection AddClientRateLimiting(this IServiceCollection services) =>
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    PerMinute(context, GlobalPermitsPerMinute));
                options.AddPolicy(ApiPolicy, context => PerMinute(context, ApiPermitsPerMinute));
                options.AddPolicy(FormPostPolicy, context => PerMinute(context, FormPostPermitsPerMinute));
                options.OnRejected = WriteRejectionAsync;
            });

        private static RateLimitPartition<string> PerMinute(HttpContext context, int permits) =>
            RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permits,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });

        private static async ValueTask WriteRejectionAsync(OnRejectedContext rejected, CancellationToken cancellationToken)
        {
            var context = rejected.HttpContext;
            int? retryAfterSeconds = null;
            if (rejected.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                retryAfterSeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
                context.Response.Headers.RetryAfter = retryAfterSeconds.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var problemDetails = context.RequestServices.GetRequiredService<IProblemDetailsService>();
                var written = await problemDetails.TryWriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too many requests.",
                        Detail = "The request rate limit was exceeded. Retry after the number of seconds in the Retry-After header.",
                        Extensions = { ["code"] = "rate_limited" }
                    }
                });
                if (written)
                {
                    return;
                }
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(FriendlyPage(retryAfterSeconds), cancellationToken);
        }

        private static string FriendlyPage(int? retryAfterSeconds)
        {
            var wait = retryAfterSeconds is { } seconds
                ? $"Please wait {seconds.ToString(CultureInfo.InvariantCulture)} seconds and try again."
                : "Please wait a moment and try again.";
            return $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="utf-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                    <title>Too many requests - EmployeeApi</title>
                    <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />
                </head>
                <body class="container py-5">
                    <h1 class="h3">Too many requests</h1>
                    <p>You have sent a lot of requests in a short time. {{wait}}</p>
                    <a href="/">Back to the home page</a>
                </body>
                </html>
                """;
        }
    }
}
