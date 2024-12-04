using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerController: ControllerBase
    {
        private readonly ILogger<EmployerController> _logger;
        private readonly IEmployerServices _employerServices;

        public EmployerController(ILogger<EmployerController> logger, IEmployerServices employerServices)
        {
            _logger = logger;
            _employerServices = employerServices;
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetAllEmployers")]
        public async Task<IActionResult> GetAllEmployersAsync()
        {
            try
            {
                var employers = await _employerServices.GetAllEmployersAsync();

                if (employers == null || employers.Count <= 0)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No data found"
                    });
                }

                return Ok(new ApiResponse<List<Employer>> { Data = employers, Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while fetching Employers."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetEmployerByUserName/{userName}")]
        public async Task<IActionResult> GetEmployerByUserNameAsync(string userName)
        {
            try
            {
                var employer = await _employerServices.GetEmployerByUserName(userName);


                if (employer == null)
                {
                    _logger.LogError("Employer not found with given username");
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Employer' with username: {userName} not found"
                    });
                }

                return Ok( new ApiResponse<Employer> { Data = employer, Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while fetching Employer."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpPut]
        [Route("UpdateEmployer/{employerId}")]
        public async Task<ActionResult<bool>> UpdateEmployer(string employerId, [FromBody] Employer employer)
        {
            try
            {
                if (employer == null)
                {
                    _logger.LogWarning("Bad Request");
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid Data"
                    });
                }

                var employerStatus = await _employerServices.UpdateEmployerAsync(employerId, employer);

                if (employerStatus.Message == "Employer not found with the given ID.")
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = employerStatus.Message
                    });
                }
                else
                {
                    return Ok(new ApiResponse<Employer>
                    {
                        Success = true,
                        Message = "Employer updated successfully",
                        Data = employer
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while updating JobSeeker."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpDelete]
        [Route("DeleteEmployer/{employerId}")]
        public async Task<IActionResult> DeleteEmployerAsync(string employerId)
        {
            try
            {
                var deleteStatus = await _employerServices.DeleteEmployerAsync(employerId);
                return Ok(new ApiResponse<string> { Message = deleteStatus.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while deleting Employer."
                });
            }
        }
    }
}
