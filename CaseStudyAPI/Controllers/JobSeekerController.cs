using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobSeekerController : ControllerBase
    {
        private readonly IJobSeekerServices _jobseekerServices;
        private readonly ILogger<JobSeekerController> _logger;

        public JobSeekerController(IJobSeekerServices jobseekerServices, ILogger<JobSeekerController> logger)
        {
            _jobseekerServices = jobseekerServices;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllJobSeekers")]
        public async Task<IActionResult> GetAllJobSeekersAsync()
        {
            try
            {
                var jobSeekers = await _jobseekerServices.GetAllJobSeekersAsync();

                if (jobSeekers == null || jobSeekers.Count <= 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No data found"
                    });
                }

                return Ok(new ApiResponse<List<JobSeeker>> { Success = true, Data = jobSeekers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while fetching JobSeekers." });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPut]
        [Route("UpdateJobSeeker/{jobSeekerId}")]
        public async Task<ActionResult<bool>> UpdateJobSeekerAsync(string jobSeekerId, [FromBody] JobSeeker jobseekers)
        {
            try
            {
                if (jobseekers == null)
                {
                    _logger.LogWarning("Bad Request");
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid Data"
                    });
                }

                var jobSeeker = await _jobseekerServices.UpdateJobSeekerAsync(jobSeekerId, jobseekers);

                if (jobSeeker.Message == "Job Seeker not found with the given ID.")
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = jobSeeker.Message
                    });
                }
                else
                {
                    return Ok(new ApiResponse<JobSeeker>
                    {
                        Success = true,
                        Message = "JobSeeker updated successfully",
                        Data = jobseekers
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

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetJobSeekerByUserName/{userName}")]
        public async Task<IActionResult> GetJobSeekerByUserNameAsync(string userName)
        {
            try
            {
                var jobSeeker = await _jobseekerServices.GetJobSeekerByUserNameAsync(userName);


                if (jobSeeker == null)
                {
                    _logger.LogError("JobSeeker not found with given username");
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'JobSeeker' with username: {userName} not found"
                    });
                }

                return Ok( new ApiResponse<JobSeeker> { Data = jobSeeker, Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while fetching JobSeeker."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpDelete]
        [Route("DeleteJobSeeker/{jobSeekerId}")]
        public async Task<IActionResult> DeleteJobSeekerAsync(string jobSeekerId)
        {
            try
            {
                var deleteStatus = await _jobseekerServices.DeleteJobSeekerAsync(jobSeekerId);
                return Ok(new ApiResponse<string> { Message = deleteStatus.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Error = "An error occurred while deleting JobSeeker."
                });
            }
        }
    }
}
