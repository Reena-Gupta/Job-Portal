using CPP_Dapper_WebAPI.Models;
using JobPortalWebAPI.JobApplicationRepository;
using JobPortalWebAPI.JobSeekerRepo;
using JobPortalWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplication _jobapp;
        public JobApplicationController(IJobApplication jobapp)
        {
            _jobapp = jobapp;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetJobApplicationByEmail( string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            try
            {
                // Fetch job applications for the provided email
                var jobApplications = await _jobapp.GetJobApplicationByEmailAsync(email);

                if (jobApplications == null)
                {
                    return NotFound("No job applications found for the provided email");
                }

                return Ok(jobApplications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{remail}")]
        public async Task<ActionResult<JobApplication>> GetJobAppByEmail([FromRoute] string remail)
        {
            try
            {
                var jobapp = await _jobapp.GetJobApplicationAsync(remail);
                if (jobapp == null)
                {
                    //_logger.LogError("Not found... {@JobPostings}");
                    return NotFound(new { message = $"Job Posting with recruiteremail {remail} not found." });
                }
                return Ok(jobapp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPatch("{recruitemail}/{job_title}/{status}")]
        //[HttpPatch("~/jobposting/update/{jobId}")]
        public async Task<IActionResult> UpdateJobapplicaton([FromRoute] string recruitemail, [FromRoute] string job_title, [FromRoute] string status)
        {
            try
            {
                var result = await _jobapp.UpdateJobapplicationAsync(recruitemail, job_title, status);
                if (result)
                    return Ok("Job Posting updated successfully.");
                else
                    //_logger.LogError("Bad Request... {@JobPostings}");
                return BadRequest("Failed to update job posting.");
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
