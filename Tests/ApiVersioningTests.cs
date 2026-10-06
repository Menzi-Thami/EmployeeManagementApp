using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementApp.Domain.Models;
using NSubstitute;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class ApiVersioningTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        [Fact]
        public async Task V1Route_ReturnsTheJobTitle_AndReportsTheSupportedVersions()
        {
            factory.JobTitleRepository.GetJobTitleByIdAsync(7, Arg.Any<CancellationToken>())
                .Returns(new JobTitles { Id = 7, JobTitle = "Developer" });

            var response = await factory.CreateClient().GetAsync("/api/v1/JobTitle/7");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).ShouldBe("{\"jobTitleName\":\"Developer\"}");
            response.Headers.GetValues("api-supported-versions").Single().ShouldBe("1.0");
        }

        [Fact]
        public async Task UnversionedRoute_IsGone()
        {
            var response = await factory.CreateClient().GetAsync("/api/JobTitle/7");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UnknownVersion_IsRejectedWithProblemDetails()
        {
            var response = await factory.CreateClient().GetAsync("/api/v2/JobTitle/7");

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
        }

        [Fact]
        public async Task AddEmployeePage_CallsTheV1Route_AndPageRoutesAreNotVersioned()
        {
            var response = await factory.CreateClient().GetAsync("/Home/AddEmployee");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Headers.Contains("api-supported-versions").ShouldBeFalse();
            (await response.Content.ReadAsStringAsync()).ShouldContain("url: '/api/v1/JobTitle/'");
        }
    }
}
