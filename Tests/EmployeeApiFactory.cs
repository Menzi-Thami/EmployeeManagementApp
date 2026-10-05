using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace EmployeeManagementApp.UnitTests
{
    /// <summary>
    /// Runs the real EmployeeApi pipeline (middleware, routing, views) in memory, with the
    /// data-facing services substituted so no database is needed.
    /// </summary>
    public sealed class EmployeeApiFactory : WebApplicationFactory<Program>
    {
        public IJobTitleRepository JobTitleRepository { get; } = Substitute.For<IJobTitleRepository>();
        public IEmployeeService EmployeeService { get; } = Substitute.For<IEmployeeService>();
        public IProjectService ProjectService { get; } = Substitute.For<IProjectService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(JobTitleRepository);
                services.AddSingleton(EmployeeService);
                services.AddSingleton(ProjectService);
            });
        }
    }
}
