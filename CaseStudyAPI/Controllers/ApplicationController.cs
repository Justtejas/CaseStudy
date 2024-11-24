using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationServices _applicationServices;
        private readonly ILogger<ApplicationController> _logger;
        public ApplicationController(IApplicationServices applicationServices , ILogger<ApplicationController> logger)
        {
            _applicationServices = applicationServices;
            _logger = logger;
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetAllApplications")]
        public async Task<IActionResult> GetAllApplicationsAsync()
        {
            try
            {
                var applications = await _applicationServices.GetAllApplicationsAsync();

                if (applications == null || applications.Count <= 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No applications found"
                    });
                }

                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Applications."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetApplicationById/{applicationId}")]
        public async Task<IActionResult> GetApplicationByIdAsync(string applicationId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByIdAsync(applicationId);
                if (application == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The Application with id: {applicationId} not found"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = application
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching application."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        [Route("GetApplicationByJSId/{jobSeekerId}")]
        public async Task<IActionResult> GetApplicationByJSIdAsync(string jobSeekerId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByJSIdAsync(jobSeekerId);

                if (application.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The Application with id: {jobSeekerId} not found"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = application
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Application."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetApplicationByEmployerId/{employerId}")]
        public async Task<IActionResult> GetApplicationByEmployerIDAsync(string employerId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByEmployerIDAsync(employerId);
                if (application != null && !application.Any())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The Application with id: {employerId} not found"
                    });
                }
                else if(application== null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The Application with id: {employerId} not found"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        data = application
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Application."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetApplicationByListingId/{jobListingId}")]
        public async Task<IActionResult> GetApplicationByJobListingIdAsync(string jobListingId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByJobListingIdAsync(jobListingId);
                if (application.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The Application with id: {jobListingId} not found"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = application
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Application."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        [Route("CreateApplication")]
        public async Task<IActionResult> CreateApplicationAsync([FromBody] ApplicationDTO applicationData)
        {
            try
            {
                if (applicationData == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid Request Body"
                    });
                }
                var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var application = await _applicationServices.GetApplicationByJSIdAsync(jobSeekerId);
                if (application != null && application.Any(a => a.JobListingId == applicationData.JobListingId))
                {
                    return StatusCode(400, "You have already applied to the Job");
                }
                var createdApplication = await _applicationServices.CreateApplicationAsync(applicationData, jobSeekerId );
                return Ok(new
                {
                    success = true,
                    data = createdApplication
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while creating Application."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpPut]
        [Route("UpdateApplication/{applicationId}")]
        public async Task<IActionResult> UpdateApplicationAsync(string applicationId,[FromBody] string applicationStatus)
        {
            try
            {
                var application = await _applicationServices.UpdateApplicationAsync(applicationId,applicationStatus);
                if (application == false)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Application not found with given id: {applicationId} "
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Application status updated to : {applicationStatus}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while updating Application."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpDelete]
        [Route("DeleteApplication/{applicationId}")]
        public async Task<IActionResult> DeleteApplicationAsync(string applicationId)
        {
            try
            {
                var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var application = await _applicationServices.DeleteApplicationAsync(applicationId,jobSeekerId);
                if (application == false)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid Application Id"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Application deleted successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while deleting Application."
                });
            }
        }
    }
}
