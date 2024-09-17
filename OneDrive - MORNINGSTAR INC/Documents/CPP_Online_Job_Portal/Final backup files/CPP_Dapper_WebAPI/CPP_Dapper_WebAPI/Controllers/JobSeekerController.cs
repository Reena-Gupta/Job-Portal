using CPP_Dapper_WebAPI.Models;
using JobPortalWebAPI.JobSeekerRepo;
using JobPortalWebAPI.LoginRequest;
using JobPortalWebAPI.Models;
using JobPortalWebAPI.RegisterReqRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobSeekerController : ControllerBase
    {
        private readonly IJobSeeker _jobseek;
        private readonly ILoginRequest _login;
        private readonly ILogger<JobSeekerController> _logger;

        //private readonly IRegisterRepo _register;
        public JobSeekerController(IJobSeeker jobseek, ILoginRequest request, ILogger<JobSeekerController> logger)
        {
            _jobseek = jobseek;
            _login = request;
            _logger = logger;
           // _register = register;
        }


        //[HttpGet("{id}")]
        //public async Task<ActionResult<Jobseeker>> GetJobSeekerById(int id)
        //{
        //    try
        //    {
        //        var jobApplication = await _jobseek.GetJobSeekerAsync(id);
        //        if (jobApplication == null)
        //        {
        //            return NotFound(new { message = $"Job Seeker with ID {id} not found." });
        //        }
        //        return Ok(jobApplication);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        //    }
        //}

        [HttpGet]
        public async Task<ActionResult<Jobseeker>> GetJobSeekerByEmail([FromQuery] string email)
        {
            try
            {
                var jobApplication = await _jobseek.GetJobSeekerByEmailAsync(email);
                if (jobApplication == null)
                {
                    _logger.LogWarning("Job Seeker with email {Email} not found.", email);
                    return NotFound(new { message = $"Job Seeker with email {email} not found." });
                }
                _logger.LogInformation("Job Seeker with email {Email} found successfully.", email);
                return Ok(jobApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving job seeker with email: {Email}", email);
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddJobseeker([FromBody] Jobseeker jobseeker)
        {
            try
            {
                var result = await _jobseek.InsertJobseekerAsync(jobseeker);
                if (result)
                    return Ok("Job Seeker inserted successfully.");
                else
                    return BadRequest("Failed to insert job seeker.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJobSeekerByEmailAsync([FromBody] Jobseeker request)
        {
            if (request == null)
            {
                _logger.LogWarning("UpdateJobSeekerByEmailAsync called with null request.");

                return BadRequest("Invalid request parameters.");
            }

            try
            {
              bool result = await _jobseek.UpdateJobseekerByEmail(request);

                if (result)
                {
                    _logger.LogInformation("Jobseeker with email {Email} updated successfully.", request.email);
                    return Ok("Jobseeker updated successfully.");
                }
                else
                {
                    _logger.LogWarning("Jobseeker with email {Email} not found for update.", request.email);
                    return NotFound("Jobseeker not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating job seeker with email {Email}.", request.email);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteJobseeker([FromRoute] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            try
            {
                bool result = await _jobseek.DeleteJobseekerByEmailAsync(email);

                if (result)
                {
                    _logger.LogInformation("Jobseeker with email {Email} deleted successfully.", email);
                    return Ok(new { message = "Jobseeker deleted successfully." });
                }
                else
                {
                    _logger.LogWarning("Jobseeker with email {Email} not found for deletion.", email);
                    return NotFound(new { message = "Jobseeker not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting jobseeker with email: {Email}", email);
                return StatusCode(500, new { message = "An error occurred while deleting the jobseeker.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null");
            }

            var jobseeker = await _login.GetJobSeekerAsync(request.Email);
            var msg = "Invalid user";
            if (jobseeker == null)
            {
                 _logger.LogWarning("Jobseeker not found with email: {Email}", request.Email);
            return NotFound("Jobseeker not found with the provided email");
               
            }

            if (jobseeker.password == request.Password)
            {
                msg = "Login successful";
                 
            }
            return Ok(new
            {
                message = msg,
                jobseeker = new
                {
                    Email = jobseeker.email,

                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterJobseekers([FromBody] RegisterReq request)
        {
            if (request == null)
            {
                _logger.LogWarning("RegisterJobseekers request body is null.");
                return BadRequest("Request body cannot be null");
            }

            try
            {
                var jobseeker = new Jobseeker
                {
                    name = request.UserName,
                    password = request.Password, // Ensure this is hashed
                    email = request.Email,
                    //Role = request.Role
                };
                _logger.LogInformation("Processing registration for role: {Role}", request.Role);

                bool isAdded = false;
                if(request.Role == "Jobseeker")
                {
                    isAdded = await _jobseek.RegisterJobseeker(jobseeker);
                }
                else if (request.Role == "Employer" )
                {
                    
                }
                else
                {
                    //write a logic for  admin
                }

                if (!isAdded)
                {
                    _logger.LogError("Failed to register jobseeker with email: {Email}", request.Email);
                    return StatusCode(500, new { message = "An error occurred while registering the jobseeker" });
                }
                _logger.LogInformation("Registration successful for email: {Email}", request.Email);
                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<Jobseeker>> Agetalljs()
        {
            try
            {
                var js = await _jobseek.getalljs();
                if (js == null)
                {
                    _logger.LogError("Not found... {@Jobseeker}");
                    return NotFound(new { message = "Jobseeker db not found." });
                }
                else
                    return Ok(js);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

    }
}
