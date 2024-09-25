using EmployeeManagementApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeApi.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class JobTitleController : ControllerBase 
    {
        private readonly IJobTitleRepository _jobTitleRepository;

        // Constructor to inject the repository
        public JobTitleController(IJobTitleRepository jobTitleRepository)
        {
            _jobTitleRepository = jobTitleRepository;
        }

        [HttpGet("{jobTitleId}")] 
        public async Task<IActionResult> GetJobTitle(int jobTitleId)
        {
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId); 
            if (jobTitle != null)
            {
                return Ok(new { jobTitleName = jobTitle.JobTitleName }); 
            }
            return NotFound(); 
        }
    }
}
