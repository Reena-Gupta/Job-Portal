using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using CPP_Dapper_WebAPI.JobPosting_Repository;
using CPP_Dapper_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace CPP_Dapper_WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobPostingsController : ControllerBase
    {
        private readonly IJobPostings _jobPostingsRepository;
        private readonly ILogger<JobPostingsController> _logger;
        private readonly IConfiguration _config;

        public JobPostingsController(IJobPostings jobPostingsRepository, ILogger<JobPostingsController> logger, IConfiguration config)
        {
            _jobPostingsRepository = jobPostingsRepository;
            _logger = logger;
            _config = config;
        }

        //[HttpGet("~/jobposting/get/{id}")]
        [HttpGet("{recruiteremail}")]
        public async Task<ActionResult<JobPostings>> GetJobPostingById([FromRoute]string recruiteremail)
        {
            try
            {
                var jobPosting = await _jobPostingsRepository.GetJobPostingsAsync(recruiteremail);
                if (jobPosting == null)
                {
                    _logger.LogError("Not found... {@JobPostings}");
                    return NotFound(new { message = $"Job Posting with recruiteremail {recruiteremail} not found." });
                }
                return Ok(jobPosting);
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        //[HttpPost("~/jobposting/insert")]
        //[HttpPost("~/add")]
        [HttpPost]
        public async Task<IActionResult> AddJobPosting([FromBody] JobPostings jobPosting)
        {
            try
            {
                var result = await _jobPostingsRepository.InsertJobPostingAsync(jobPosting);
                if (result)
                    return Ok("Job Posting inserted successfully.");
                else
                _logger.LogError("Bad Request... {@JobPostings}");
                return BadRequest("Failed to insert job posting.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{recruitemail}/{job_title}")]
        //[HttpPatch("~/jobposting/update/{jobId}")]
        public async Task<IActionResult> UpdateJobPosting([FromRoute]string recruitemail, [FromRoute] string job_title, [FromBody] JobPostings jobPosting)
        {
            try
            {
                var result = await _jobPostingsRepository.UpdateJobPostingAsync(recruitemail, job_title, jobPosting);
                if (result)
                    return Ok("Job Posting updated successfully.");
                else
                 _logger.LogError("Bad Request... {@JobPostings}");
                return BadRequest("Failed to update job posting.");
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{job_title}/{remail}")]
        public async Task<IActionResult> DeleteJobPosting([FromRoute] string job_title, [FromRoute] string remail)
        {
            try
            {
                var result = await _jobPostingsRepository.DeleteJobPostingAsync(job_title, remail);
                if (result)
                    return Ok("Job Posting deleted successfully.");
                else
                    _logger.LogError("Bad Request... {@JobPostings}");
                return BadRequest("Failed to deleted job posting.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<JobPostings>> Agetalljobpostings()
        {
            try
            {
                var recruit = await _jobPostingsRepository.getalljobpostings();
                if (recruit == null)
                {
                    _logger.LogError("Not found... {@Recruiters}");
                    return NotFound(new { message = "Job psoting db not found." });
                }
                return Ok(recruit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Dictionary<string, object>>>> GetAllJobPostings()
        {
            try
            {
                var jobPostings = await _jobPostingsRepository.GetAllJobPostingsAsync();

                if (jobPostings == null || !jobPostings.Any())
                {
                    return NotFound("No job postings found.");
                }

                // Filter properties that are not null
                var filteredResults = jobPostings.Select(jobPosting =>
                {
                    // Use reflection to get all properties
                    var properties = jobPosting.GetType().GetProperties();

                    // Convert to dictionary and filter out null values
                    var filteredDict = properties
                        .Where(prop => prop.GetValue(jobPosting) != null)  // Only include non-null properties
                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(jobPosting));

                    return filteredDict;
                }).ToList();

                return Ok(filteredResults);  // Return the filtered dictionary
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


    }
}
