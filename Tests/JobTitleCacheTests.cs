using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class JobTitleCacheTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;
        private readonly IJobTitleRepository _inner = Substitute.For<IJobTitleRepository>();
        private readonly CachedJobTitleRepository _sut;

        public JobTitleCacheTests()
        {
            // A real HybridCache (in-memory L1), fresh per test.
            var cache = new ServiceCollection().AddHybridCache().Services
                .BuildServiceProvider().GetRequiredService<HybridCache>();
            _sut = new CachedJobTitleRepository(_inner, cache);
            _inner.GetAllJobTitlesAsync(Arg.Any<CancellationToken>()).Returns(
            [
                new JobTitles { Id = 1, JobTitle = "Developer" },
                new JobTitles { Id = 2, JobTitle = "DBA" }
            ]);
        }

        [Fact]
        public async Task GetAll_SecondCall_IsServedFromTheCache()
        {
            var first = (await _sut.GetAllJobTitlesAsync(_ct)).ToList();
            var second = (await _sut.GetAllJobTitlesAsync(_ct)).ToList();

            second.Select(j => j.JobTitle).ShouldBe(first.Select(j => j.JobTitle));
            await _inner.Received(1).GetAllJobTitlesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetById_IsAnsweredFromTheOneCachedList()
        {
            (await _sut.GetJobTitleByIdAsync(1, _ct))!.JobTitle.ShouldBe("Developer");
            (await _sut.GetJobTitleByIdAsync(2, _ct))!.JobTitle.ShouldBe("DBA");
            (await _sut.GetJobTitleByIdAsync(99, _ct)).ShouldBeNull();

            await _inner.Received(1).GetAllJobTitlesAsync(Arg.Any<CancellationToken>());
            await _inner.DidNotReceive().GetJobTitleByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AFailedLoad_IsNotCached()
        {
            _inner.GetAllJobTitlesAsync(Arg.Any<CancellationToken>()).Returns(
                _ => throw new InvalidOperationException("database down"),
                _ => Task.FromResult<System.Collections.Generic.IEnumerable<JobTitles>>([new JobTitles { Id = 1, JobTitle = "Developer" }]));

            await Should.ThrowAsync<InvalidOperationException>(() => _sut.GetAllJobTitlesAsync(_ct));
            var retried = await _sut.GetJobTitleByIdAsync(1, _ct);

            retried!.JobTitle.ShouldBe("Developer");
            await _inner.Received(2).GetAllJobTitlesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ACallerMutatingTheResult_DoesNotChangeTheCachedEntry()
        {
            var jobTitle = await _sut.GetJobTitleByIdAsync(1, _ct);
            jobTitle!.JobTitle = "Changed by a caller";

            (await _sut.GetJobTitleByIdAsync(1, _ct))!.JobTitle.ShouldBe("Developer");
        }

        [Fact]
        public void TheApp_ResolvesTheCachedRepository()
        {
            using var app = new WebApplicationFactory<Program>();
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetRequiredService<IJobTitleRepository>().ShouldBeOfType<CachedJobTitleRepository>();
            CachedJobTitleRepository.EntryOptions.Expiration.ShouldNotBeNull();
        }
    }
}
