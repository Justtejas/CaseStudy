using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationServices;
        private readonly IEmployerServices _employerServices;
        private readonly IJobSeekerServices _jobseekerServices;
        private readonly IUserServices _userServices;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthorizationService authorizationServices, IEmployerServices employerServices, IJobSeekerServices jobseekerServices, IUserServices userServices, ILogger<AuthController> logger)
        {
            _authorizationServices = authorizationServices;
            _userServices = userServices;
            _employerServices = employerServices;
            _jobseekerServices = jobseekerServices;
            _logger = logger;
        }
        [Route("employer/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDTO registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid Data" });
            }
            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Passwords do not Match" }); ;
            }
            var createdUser = await _userServices.RegisterEmployerAsync(registrationData);
            if (createdUser != null)
            {
                return Ok(new
                {
                    success = true,
                    data = createdUser
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    ModelState
                });
            }
        }
        [Route("jobseeker/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterJobSeeker([FromBody] RegisterJobSeekerDTO registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid Data" });
            }

            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Passwords do not Match" }); ;
            }
            var createdUser = await _userServices.RegisterJobSeekerAsync(registrationData);
            if (createdUser != null)
            {
                if (createdUser.Status == "Success")
                {
                    return Ok(createdUser.Message);
                }
                else
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = createdUser.Message });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                ApiResponse<string>
                { Success = false, Error = "An error occurred." });
            }
        }
        [Route("employer/login")]
        [HttpPost]
        public async Task<IActionResult> EmployerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(new ApiResponse<string> { Success = false, Message = "Request Body Cannot Be Null" });
            var employer = await _employerServices.GetEmployerByUserName(login.UserName);
            if (employer == null) return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid User Name!" });
            var match = _authorizationServices.VerifyPassword(login.Password, employer.Password);
            if (!match) return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid Password!" });
            var token = _userServices.Login(employer);
            if (token == null) return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            try
            {
                var cookie = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = token.Expiration,
                    MaxAge = TimeSpan.FromDays(10)
                };
                Response.Cookies.Append("jwt", token.Token, cookie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            }

            var user = new
            {
                employer.EmployerId,
                employer.EmployerName,
                employer.UserName,
            };
            return Ok(new { user, token.Token });
        }

        [Route("jobseeker/login")]
        [HttpPost]
        public async Task<IActionResult> JobSeekerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(new ApiResponse<string> { Success = false, Message = "Request Body Cannot Be Null" }); ;
            var jobSeeker = await _jobseekerServices.GetJobSeekerByUserNameAsync(login.UserName);
            if (jobSeeker == null) return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid User Name!" });
            var match = _authorizationServices.VerifyPassword(login.Password, jobSeeker.Password);
            if (!match) return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid Password!" });
            var token = _userServices.Login(jobSeeker);
            if (token == null) return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            try
            {
                var cookie = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    MaxAge = TimeSpan.FromDays(10)
                };
                Response.Cookies.Append("jwt", token.Token, cookie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            }
            var user = new
            {
                jobSeeker.JobSeekerId,
                jobSeeker.JobSeekerName,
                jobSeeker.UserName,
            };
            return Ok(new { user, token.Token });
        }
        [Route("logout")]
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("jwt");
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "Successfully logged out."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while logging out."
                });
            }

        }
    }
}
