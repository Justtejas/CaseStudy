using CaseStudyAPI.Data;
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
    [Authorize(Roles = "Employer,JobSeeker")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationServices _applicationServices;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApplicationServices applicationServices, ILogger<ApplicationController> logger)
        {
            _applicationServices = applicationServices;
            _logger = logger;
        }

        private IActionResult HandleException(Exception ex, string action)
        {
            _logger.LogError(ex, $"Error occurred during {action}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
            {
                Success = false,
                Error = $"An error occurred while processing your request: {action}."
            });
        }

        private string GetUserClaim(string claimType)
        {
            return User.FindFirstValue(claimType);
        }

        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAllApplicationsAsync()
        {
            try
            {
                var applications = await _applicationServices.GetAllApplicationsAsync();
                if (applications == null || !applications.Any())
                {
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No applications found" });
                }

                return Ok(new ApiResponse<List<Application>>
                {
                    Success = true,
                    Data = applications
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "fetching all applications");
            }
        }

        [HttpGet("GetApplicationById/{applicationId}")]
        public async Task<IActionResult> GetApplicationByIdAsync(string applicationId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByIdAsync(applicationId);
                if (application == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Application with ID {applicationId} not found."
                    });
                }

                return Ok(new ApiResponse<Application>
                {
                    Success = true,
                    Data = application
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "fetching application by ID");
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpGet("GetApplicationByJSId/{jobSeekerId}")]
        public async Task<IActionResult> GetApplicationByJSIdAsync(string jobSeekerId)
        {
            try
            {
                var applications = await _applicationServices.GetApplicationByJSIdAsync(jobSeekerId);
                if (applications == null || !applications.Any())
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"No applications found for JobSeeker ID {jobSeekerId}."
                    });
                }

                return Ok(new ApiResponse<List<Application>>
                {
                    Success = true,
                    Data = applications
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "fetching applications by JobSeeker ID");
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet("GetApplicationByEmployerId/{employerId}")]
        public async Task<IActionResult> GetApplicationByEmployerIDAsync(string employerId)
        {
            try
            {
                var applications = await _applicationServices.GetApplicationByEmployerIDAsync(employerId);
                if (applications == null || !applications.Any())
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"No applications found for Employer ID {employerId}."
                    });
                }

                return Ok(new ApiResponse<List<Application>>
                {
                    Success = true,
                    Data = applications
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "fetching applications by Employer ID");
            }
        }

        [HttpGet("GetApplicationByListingId/{jobListingId}")]
        public async Task<IActionResult> GetApplicationByJobListingIdAsync(string jobListingId)
        {
            try
            {
                var applications = await _applicationServices.GetApplicationByJobListingIdAsync(jobListingId);
                if (applications == null || !applications.Any())
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"No applications found for Job Listing ID {jobListingId}."
                    });
                }

                return Ok(new ApiResponse<List<Application>>
                {
                    Success = true,
                    Data = applications
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "fetching applications by Job Listing ID");
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost("CreateApplication")]
        public async Task<IActionResult> CreateApplicationAsync([FromBody] ApplicationDTO applicationData)
        {
            try
            {
                if (applicationData == null)
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid request body" });
                }

                var jobSeekerId = GetUserClaim(ClaimTypes.NameIdentifier);
                var existingApplications = await _applicationServices.GetApplicationByJSIdAsync(jobSeekerId);
                if (existingApplications.Any(a => a.JobListingId == applicationData.JobListingId))
                {
                    return Conflict(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "You have already applied to this job."
                    });
                }

                var createdApplication = await _applicationServices.CreateApplicationAsync(applicationData, jobSeekerId);
                return Ok(new ApiResponse<Application>
                {
                    Success = true,
                    Data = createdApplication
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "creating application");
            }
        }

        [HttpPut("UpdateApplication/{applicationId}/{applicationStatus}")]
        public async Task<IActionResult> UpdateApplicationAsync(string applicationId, string applicationStatus)
        {
            try
            {
                var success = await _applicationServices.UpdateApplicationAsync(applicationId, applicationStatus);
                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Application with ID {applicationId} not found."
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = $"Application status updated to: {applicationStatus}"
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "updating application status");
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpDelete("DeleteApplication/{applicationId}")]
        public async Task<IActionResult> DeleteApplicationAsync(string applicationId)
        {
            try
            {
                var jobSeekerId = GetUserClaim(ClaimTypes.NameIdentifier);
                var success = await _applicationServices.DeleteApplicationAsync(applicationId, jobSeekerId);
                if (!success)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid application ID."
                    });
               }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Application deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "deleting application");
            }
        }
    }
}
