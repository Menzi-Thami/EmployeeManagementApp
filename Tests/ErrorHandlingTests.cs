using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.Common.Exceptions;
using EmployeeManagementApp.Application.DTOs;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class ErrorHandlingTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        [Fact]
        public async Task ApiRoute_NotFoundException_Returns404ProblemDetails()
        {
            factory.JobTitleRepository.GetJobTitleByIdAsync(404, Arg.Any<CancellationToken>())
                .ThrowsAsync(new NotFoundException("Job title 404 was not found."));
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/v1/JobTitle/404");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
            var body = await response.Content.ReadAsStringAsync();
            body.ShouldContain("\"status\":404");
            body.ShouldContain("\"traceId\"");
        }

        [Fact]
        public async Task ApiRoute_UnexpectedException_Returns500WithoutExceptionText()
        {
            factory.JobTitleRepository.GetJobTitleByIdAsync(500, Arg.Any<CancellationToken>())
                .ThrowsAsync(new ArgumentException("driver detail that must not leak"));
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/v1/JobTitle/500");

            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
            (await response.Content.ReadAsStringAsync()).ShouldNotContain("driver detail");
        }

        [Fact]
        public async Task MvcPage_UnexpectedException_RendersTheErrorViewNotJson()
        {
            factory.EmployeeService.GetAllEmployeesAsync(Arg.Any<CancellationToken>())
                .ThrowsAsync(new InvalidOperationException("boom"));
            var client = factory.CreateClient();

            var response = await client.GetAsync("/Home/ViewEmployees");

            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
            (await response.Content.ReadAsStringAsync()).ShouldContain("An error occurred while processing your request.");
        }

        [Fact]
        public async Task AddEmployeePost_WithUnknownJobTitle_ShowsTheErrorOnTheForm()
        {
            factory.EmployeeService.AddEmployeeAsync(Arg.Is<EmployeeDto>(e => e.Name == "Unknown"), Arg.Any<CancellationToken>())
                .ThrowsAsync(new ValidationException(nameof(EmployeeDto.JobTitleId), "Job title 4 does not exist."));
            var client = factory.CreateClient();
            var form = await AntiforgeryTests.FormWithTokenAsync(client);
            form["Name"] = "Unknown";
            form["Surname"] = "Person";
            form["JobTitleId"] = "4";
            // The form's read-only field; the page's script fills it, even with "Job Title not found".
            form["JobTitleName"] = "Job Title not found";
            form["DateOfBirth"] = "1990-05-01";

            var response = await client.PostAsync("/Home/AddEmployee", new FormUrlEncodedContent(form));

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).ShouldContain("Job title 4 does not exist.");
        }
    }
}
