using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EmployeeApi.RateLimiting;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    // Each test builds its own app so the limiter's counters start at zero.
    public class RateLimitingTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        private WebApplicationFactory<Program> FreshApp() => factory.WithWebHostBuilder(_ => { });

        private static async Task<HttpResponseMessage> SendRepeatedlyAsync(int times, System.Func<Task<HttpResponseMessage>> send)
        {
            HttpResponseMessage? last = null;
            for (var i = 0; i < times; i++)
            {
                last?.Dispose();
                last = await send();
                last.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests, $"request {i + 1} of {times} was limited too early");
            }
            return last!;
        }

        [Fact]
        public async Task AddEmployeePost_OverTheFormLimit_Gets429HtmlPageWithRetryAfter()
        {
            using var app = FreshApp();
            var client = app.CreateClient();
            static FormUrlEncodedContent Form() => new(new Dictionary<string, string> { ["Name"] = "x" });
            await SendRepeatedlyAsync(RateLimitingExtensions.FormPostPermitsPerMinute,
                () => client.PostAsync("/Home/AddEmployee", Form()));

            var limited = await client.PostAsync("/Home/AddEmployee", Form());

            limited.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
            limited.Headers.RetryAfter.ShouldNotBeNull();
            limited.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
            (await limited.Content.ReadAsStringAsync()).ShouldContain("Too many requests");
            // The stricter policy is on the POST only; the form page itself still loads.
            (await client.GetAsync("/Home/AddEmployee")).StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Api_OverTheApiLimit_Gets429ProblemDetailsWithRetryAfter()
        {
            using var app = FreshApp();
            var client = app.CreateClient();
            await SendRepeatedlyAsync(RateLimitingExtensions.ApiPermitsPerMinute,
                () => client.GetAsync("/api/JobTitle/1"));

            var limited = await client.GetAsync("/api/JobTitle/1");

            limited.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
            limited.Headers.RetryAfter.ShouldNotBeNull();
            limited.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
            var body = await limited.Content.ReadAsStringAsync();
            body.ShouldContain("\"status\":429");
            body.ShouldContain("\"code\":\"rate_limited\"");
        }

        [Fact]
        public async Task HealthAndStaticFiles_DoNotCountAgainstTheGlobalLimit()
        {
            using var app = FreshApp();
            var client = app.CreateClient();
            var overTheLimit = RateLimitingExtensions.GlobalPermitsPerMinute + 1;

            await SendRepeatedlyAsync(overTheLimit, () => client.GetAsync("/health/live"));
            await SendRepeatedlyAsync(overTheLimit, () => client.GetAsync("/css/site.css"));

            (await client.GetAsync("/")).StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Pages_OverTheGlobalLimit_Get429()
        {
            using var app = FreshApp();
            var client = app.CreateClient();
            await SendRepeatedlyAsync(RateLimitingExtensions.GlobalPermitsPerMinute, () => client.GetAsync("/"));

            var limited = await client.GetAsync("/");

            limited.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
            limited.Headers.RetryAfter.ShouldNotBeNull();
        }
    }
}
