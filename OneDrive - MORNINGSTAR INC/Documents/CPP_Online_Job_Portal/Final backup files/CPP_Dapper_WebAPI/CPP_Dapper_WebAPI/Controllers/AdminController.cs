using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CPP_Dapper_WebAPI.Admin_Repository;
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
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _adminRepository;
        private readonly ILogger <AdminController> _logger;
        private readonly IConfiguration _configuration;
        public AdminController(IAdmin adminRepository, ILogger<AdminController> logger, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("~/admin/get/{Admin_Id}")]
        [HttpGet("{Admin_Id}")]
        public async Task<ActionResult<Admin>> GetAdminById([FromRoute] int Admin_Id)
        {
            try
            {
                var admin = await _adminRepository.GetAdminAsync(Admin_Id);
                if (admin == null)
                {
                    _logger.LogError("Not Found... {@Admin}");
                    return NotFound(new { message = $"Admin with ID {Admin_Id} not found." });
                }
                else
                    return Ok(admin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        //[HttpPost("~/admin/insert")]
        //[HttpPost("~/add")]
        [HttpPost]
        public async Task<IActionResult> AddAdmin([FromBody] Admin owner)
        {
            try
            {
                var result = await _adminRepository.InsertAdminAsync(owner);
                if (result)
                    return Ok("Admin inserted successfully.");
                else
                _logger.LogError("Bad Request... {@Admin}");
                return BadRequest("Failed to insert admin.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{Username}")]
        public async Task<ActionResult<Admin>> GetAdminbyusername([FromRoute] string Username)
        {
            try
            {
                var owner = await _adminRepository.GetAdmin(Username);
                if (owner == null)
                {
                    _logger.LogError("Not found... {@Admin}");
                    return NotFound(new { message = $"Admin with ID {Username} not found." });
                }
                // Generate JWT token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// Use a strong secret key and store it securely
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, owner.Username), // Use username for token claims
                        new Claim("Password", owner.Password) // Custom claim to include admin ID
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = signIn
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString, User = owner });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
