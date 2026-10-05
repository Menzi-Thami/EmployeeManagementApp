using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.DTOs;
using NSubstitute;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class AntiforgeryTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        // Loads the rendered AddEmployee form (which also sets the antiforgery cookie on the client)
        // and returns its hidden token field, ready for the caller to add the other fields.
        internal static async Task<Dictionary<string, string>> FormWithTokenAsync(HttpClient client)
        {
            var page = await client.GetStringAsync("/Home/AddEmployee");
            var token = Regex.Match(page, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");
            token.Success.ShouldBeTrue("the AddEmployee form should render an antiforgery token");
            return new Dictionary<string, string> { ["__RequestVerificationToken"] = token.Groups[1].Value };
        }

        private static Dictionary<string, string> ValidEmployee(Dictionary<string, string> form)
        {
            form["Name"] = "Ada";
            form["Surname"] = "Lovelace";
            form["JobTitleId"] = "1";
            form["JobTitleName"] = "Developer";
            form["DateOfBirth"] = "1990-05-01";
            return form;
        }

        [Fact]
        public async Task AddEmployeePost_WithoutAntiforgeryToken_IsRejectedAndNothingIsSaved()
        {
            var client = factory.CreateClient();

            var response = await client.PostAsync(
                "/Home/AddEmployee", new FormUrlEncodedContent(ValidEmployee(new Dictionary<string, string>())));

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            await factory.EmployeeService.DidNotReceive()
                .AddEmployeeAsync(Arg.Is<EmployeeDto>(e => e.Surname == "Lovelace"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AddEmployeePost_ThroughTheRenderedForm_IsAccepted()
        {
            var client = factory.CreateClient();
            var form = ValidEmployee(await FormWithTokenAsync(client));
            form["Surname"] = "Byron";

            var response = await client.PostAsync("/Home/AddEmployee", new FormUrlEncodedContent(form));

            // Success redirects to the list; the test client follows redirects, so check where it landed.
            response.RequestMessage!.RequestUri!.AbsolutePath.ShouldBe("/Home/ViewEmployees");
            await factory.EmployeeService.Received(1)
                .AddEmployeeAsync(Arg.Is<EmployeeDto>(e => e.Surname == "Byron"), Arg.Any<CancellationToken>());
        }
    }
}
