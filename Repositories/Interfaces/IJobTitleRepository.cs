using EmployeeManagementApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public interface IJobTitleRepository
    {
        Task<IEnumerable<JobTitles>> GetAllJobTitlesAsync();
        Task<JobTitles> GetJobTitleByIdAsync(int jobTitleId);
    }
}
