using EmployeeManagementApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public interface IJobTitleRepository
    {
        Task<IEnumerable<JobTitle>> GetAllJobTitlesAsync();
        Task<JobTitle> GetJobTitleByIdAsync(int jobTitleId);
    }
}
