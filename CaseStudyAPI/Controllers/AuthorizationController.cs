using CaseStudyAPI.DTO;
using CaseStudyAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationServices;
        private readonly IEmployerServices _employerServices;
        private readonly IJobSeekerServices _jobseekerServices;
        private readonly IUserServices _userServices;

        public AuthorizationController(IAuthorizationService authorizationServices, IEmployerServices employerServices, IJobSeekerServices jobseekerServices, IUserServices userServices)
        {
            _authorizationServices = authorizationServices;
            _userServices = userServices;
            _employerServices = employerServices;
            _jobseekerServices = jobseekerServices;
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
                    message = "Employer registered successfully.",
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
                return Ok(new
                {
                    success = true,
                    message = "JobSeeker registered successfully.",
                    data = createdUser
                });
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
            var user = await _employerServices.GetEmployerByUserName(login.UserName);
            if (user == null) return BadRequest("Invalid UserName or Password!");
            var match = await _authorizationServices.VerifyPasswordAsync(login.Password, user.Password);
            if (!match) return BadRequest("Invalid UserName or Password!");
            var token = await _userServices.LoginAsync(user);
            return Ok(token);
        }

        [Route("jobseeker/login")]
        [HttpPost]
        public async Task<IActionResult> JobSeekerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState); ;
            var user = await _jobseekerServices.GetJobSeekerByUserName(login.UserName);
            if (user == null) return BadRequest("Invalid UserName or Password!");
            var match = await _authorizationServices.VerifyPasswordAsync(login.Password, user.Password);
            if (!match) return BadRequest("Invalid Email or Password!");
            var token = _userServices.LoginAsync(user);
            return Ok(token);
        }
    }
}
