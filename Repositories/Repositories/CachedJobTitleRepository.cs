using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Domain.Models;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    /// <summary>
    /// Cache-aside over <see cref="JobTitleRepository"/>. Job titles are a handful of reference
    /// rows read on every AddEmployee post and every dropdown change, so the whole list is
    /// cached as one entry and lookups by id are served from it. Nothing in the app writes job
    /// titles (they are seeded by MyDB.sql), so there is no write path to invalidate from; the
    /// expiry bounds how long an out-of-band database edit takes to show up. If a write path is
    /// added, call <c>HybridCache.RemoveAsync(CacheKey)</c> after it commits.
    /// </summary>
    public sealed class CachedJobTitleRepository(IJobTitleRepository inner, HybridCache cache) : IJobTitleRepository
    {
        public const string CacheKey = "job-titles:all";

        public static readonly HybridCacheEntryOptions EntryOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(10)
        };

        public async Task<IEnumerable<JobTitles>> GetAllJobTitlesAsync(CancellationToken cancellationToken) =>
            await GetCachedAsync(cancellationToken);

        public async Task<JobTitles?> GetJobTitleByIdAsync(int jobTitleId, CancellationToken cancellationToken) =>
            (await GetCachedAsync(cancellationToken)).FirstOrDefault(jobTitle => jobTitle.Id == jobTitleId);

        // JobTitles is mutable, so HybridCache hands every caller its own deserialized copy;
        // a caller editing the result cannot change what the next caller sees.
        private ValueTask<JobTitles[]> GetCachedAsync(CancellationToken cancellationToken) =>
            cache.GetOrCreateAsync(
                CacheKey,
                inner,
                static async (repository, token) => (await repository.GetAllJobTitlesAsync(token)).ToArray(),
                EntryOptions,
                cancellationToken: cancellationToken);
    }

    public static class JobTitleServiceCollectionExtensions
    {
        /// <summary>Registers the job title repository behind the HybridCache decorator.</summary>
        public static IServiceCollection AddCachedJobTitles(this IServiceCollection services)
        {
            services.AddHybridCache();
            services.AddScoped<JobTitleRepository>();
            services.AddScoped<IJobTitleRepository>(sp => new CachedJobTitleRepository(
                sp.GetRequiredService<JobTitleRepository>(), sp.GetRequiredService<HybridCache>()));
            return services;
        }
    }
}
