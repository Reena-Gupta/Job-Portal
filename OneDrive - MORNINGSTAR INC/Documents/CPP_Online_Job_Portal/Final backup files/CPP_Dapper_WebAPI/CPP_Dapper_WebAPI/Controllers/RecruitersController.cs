using CPP_Dapper_WebAPI.JobPosting_Repository;
using CPP_Dapper_WebAPI.Models;
using CPP_Dapper_WebAPI.Recruiters_Repository;
using JobPortalWebAPI.LoginRequest;
using JobPortalWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPP_Dapper_WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecruitersController : ControllerBase
    {
        private readonly IRecruiters _recruitRepository;
        private readonly ILoginRequest _login;
        private readonly ILogger<RecruitersController> _logger;

        public RecruitersController(IRecruiters recruitRepository,ILoginRequest login ,ILogger<RecruitersController> logger)
        {
            _recruitRepository = recruitRepository;
            _logger = logger;
            _login = login;
        }

        [HttpGet("~/recruiters/get/{email}")]
        [HttpGet("{email}")]
        public async Task<ActionResult<Recruiters>> GetRecruitersByEmail([FromRoute] string email)
        {
            try
            {
                var recruit = await _recruitRepository.GetRecruitersAsync(email);
                if (recruit == null)
                {
                    _logger.LogError("Not found... {@Recruiters}");
                    return NotFound(new { message = $"Recruiter with ID {email} not found." });
                }else
                    return Ok(recruit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        //[HttpPost("~/recruiters/insert")]
        //[HttpPost("~/add")]
        [HttpPost]
        public async Task<IActionResult> AddRecruiters([FromBody] Recruiters recruit)
        {
            try
            {
                var result = await _recruitRepository.InsertRecruitersAsync(recruit);
                if (result)
                    return Ok("Recruiters inserted successfully.");
                else
                _logger.LogError("Bad Request... {@Recruiters}");
                return BadRequest("Failed to insert Recruiters.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{recruitemail}")]
        [HttpPatch("~/recruiters/update/{recruitemail}")]
        public async Task<IActionResult> UpdateRecruiters([FromRoute] string recruitemail, [FromBody] Recruiters recruit)
        {
            try
            {
                var result = await _recruitRepository.UpdateRecruitersAsync(recruitemail, recruit);
                if (result)
                    return Ok("Recruiters updated successfully.");
                else
                _logger.LogError("Bad Request... {@Recruiters}");
                return BadRequest("Failed to update Recruiters.");
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{email}")]
        [HttpDelete("~/recruiters/delete/{email}")]
        public async Task<IActionResult> DeleteRecruiters([FromRoute] string email)
        {
            try
            {
                var result = await _recruitRepository.DeleteRecruitersAsync(email);
                if (result)
                    return Ok("Recruiter Deleted successfully.");
                else
                _logger.LogError("Bad Request... {@Recruiters}");
                return BadRequest("Failed to update Recruiters");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRecruiters([FromBody] RegisterReq request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null");
            }
            try
            {
                var recruit = new Recruiters
                {
                    company_name = request.UserName,
                    password = request.Password, // Ensure this is hashed
                    email = request.Email,
                    //Role = request.Role
                };
                var isAdded = false;
                if (request.Role == "Jobseeker")
                {
                    //isAdded = await _recruitRepository.RegisterJobseeker(jobseeker);
                }
                else if (request.Role == "Employer")
                {
                    isAdded = await _recruitRepository.RegisterRecruiters(recruit);
                }
                else
                {
                    //write a logic for  admin
                }
                if (!isAdded)
                {
                    return StatusCode(500, new { message = "An error occurred while registering the jobseeker" });
                }

                // Return a success response after adding the jobseeker
                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging in production)
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null");
            }

            // Fetch the jobseeker record from the database by email
            var recruiters = await _login.GetRecruitersAsync(request.Email);
            var msg = "Invalid user";
            if (recruiters == null)
            {
                return NotFound("Recruiters not found with the provided email");
            }

            // Check if the provided password matches the stored password
            if (recruiters.password == request.Password)
            {
                msg = "Login successful";
                //Unauthorized("Invalid email or password");
            }

            // If the jobseeker is successfully authenticated, return a success message
            return Ok(new
            {
                message = msg,
                recruiters = new
                {
                    Email = recruiters.email,

                }
            });
        }
        [HttpGet]
        public async Task<ActionResult<Recruiters>> Agetallrecruiter()
        {
            try
            {
                var recruit = await _recruitRepository.getallrecruiters();
                if (recruit == null)
                {
                    _logger.LogError("Not found... {@Recruiters}");
                    return NotFound(new { message = "Recruiter db not found." });
                }
                else
                    return Ok(recruit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
