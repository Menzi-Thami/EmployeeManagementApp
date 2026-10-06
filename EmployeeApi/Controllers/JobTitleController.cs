using Asp.Versioning;
using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using EmployeeApi.RateLimiting;
using System.Threading.Tasks;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableRateLimiting(RateLimitingExtensions.ApiPolicy)]
    public class JobTitleController : ControllerBase 
    {
        private readonly IJobTitleRepository _jobTitleRepository;

        // Constructor to inject the repository
        public JobTitleController(IJobTitleRepository jobTitleRepository)
        {
            _jobTitleRepository = jobTitleRepository;
        }

        [HttpGet("{jobTitleId}")] 
        public async Task<IActionResult> GetJobTitle(int jobTitleId, CancellationToken cancellationToken)
        {
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId, cancellationToken);
            if (jobTitle != null)
            {
                return Ok(new { jobTitleName = jobTitle.JobTitle }); 
            }
            return NotFound(); 
        }
    }
}
