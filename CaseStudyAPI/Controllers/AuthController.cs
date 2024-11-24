using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [EnableCors("AllowAny")]
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
                return BadRequest(ModelState);
            }
            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "Passwords do not match!");
                return BadRequest(ModelState);
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
                ModelState.AddModelError("Error", "An error occuerd while creating the employer!");
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
                return BadRequest(ModelState);
            }

            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "Passwords do not match!");
                return BadRequest(ModelState);
            }
            var createdUser = await _userServices.RegisterJobSeekerAsync(registrationData);
            if (createdUser != null)
            {
                if(createdUser.Status == "Success")
                {
                    return Ok(new
                    {
                        data = createdUser
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        data = createdUser
                    });
                }
            }
            else
            {
                ModelState.AddModelError("Error", "An error occuerd while creating the jobseeker!");
                return StatusCode(500, new
                {
                    success = false,
                    ModelState
                });
            }
        }
        [Route("employer/login")]
        [HttpPost]
        public async Task<IActionResult> EmployerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState);
            var employer = await _employerServices.GetEmployerByUserName(login.UserName);
            if (employer == null) return BadRequest("Invalid User Name!");
            var match = await _authorizationServices.VerifyPasswordAsync(login.Password, employer.Password);
            if (!match) return BadRequest("Invalid Password!");
            var token = await _userServices.LoginAsync(employer);
            if(token == null) return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            try
            {
                var cookie = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    Expires = token.Expiration
                };
                Response.Cookies.Append("authToken", token.Token, cookie);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            }
            return Ok(new
            {
                employer = new
                {
                    employer.EmployerId,
                    employer.EmployerName,
                    employer.UserName,
                }
            });
        }

        [Route("jobseeker/login")]
        [HttpPost]
        public async Task<IActionResult> JobSeekerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState); ;
            var jobSeeker = await _jobseekerServices.GetJobSeekerByUserName(login.UserName);
            if (jobSeeker == null) return BadRequest("Invalid User Name!");
            var match = await _authorizationServices.VerifyPasswordAsync(login.Password, jobSeeker.Password);
            if (!match) return BadRequest("Invalid Password!");
            var token = await _userServices.LoginAsync(jobSeeker);
            if(token == null) return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            try
            {
                var cookie = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    Expires = token.Expiration
                };
                Response.Cookies.Append("authToken", token.Token, cookie);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred." });
            }
            return Ok(new
            {
                jobSeeker = new
                {
                    jobSeeker.JobSeekerId,
                    jobSeeker.JobSeekerName,
                    jobSeeker.UserName,
                }
            });
        }
    }
}
