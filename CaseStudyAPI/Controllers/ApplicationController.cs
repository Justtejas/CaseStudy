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
        public ApplicationController(IApplicationServices applicationServices)
        {
            _applicationServices = applicationServices;
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetAllApplications")]
        public async Task<ActionResult<List<Application>>> GetAllApplicationsAsync()
        {
            try
            {
                var applications = await _applicationServices.GetAllApplicationsAsync();

                if (applications == null || applications.Count <= 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No data found"
                    });
                }

                return Ok(applications);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Applications."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetApplicationById/{id}")]
        public async Task<ActionResult<Application>> GetApplicationByIdAsync(string applicationId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByIdAsync(applicationId);
                if (application == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Application' with Id: {applicationId} not found"
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Employer."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        [Route("GetApplicationByJSId/{id}")]
        public async Task<ActionResult<Application>> GetApplicationByJSIdAsync(string jobSeekerId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByJSIdAsync(jobSeekerId);

                if (application.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Application' with Id: {jobSeekerId} not found"
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Application."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetApplicationByEmployerId/{id}")]
        public async Task<ActionResult<Application>> GetApplicationByEmployerIDAsync(string employerId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByEmployerIDAsync(employerId);
                if (application != null && !application.Any())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Application' with Id: {employerId} not found"
                    });
                }
                else if(application== null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Application' with Id: {employerId} not found"
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Application."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetApplicationByListingId/{id}")]
        public async Task<ActionResult<Application>> GetApplicationByJobListingIdAsync(string jobListingId)
        {
            try
            {
                var application = await _applicationServices.GetApplicationByJobListingIdAsync(jobListingId);
                if (application.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Application' with Id: {jobListingId} not found"
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
        public async Task<ActionResult<Application>> CreateApplicationAsync([FromBody] Application model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Null Object"
                    });
                }
                model.ApplicationId = Guid.NewGuid().ToString();
                model.ApplicationDate = DateTime.Now;
                model.JobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var application = await _applicationServices.GetApplicationByJSIdAsync(model.JobSeekerId);
                if (application != null && application.Any(a => a.JobListingId == model.JobListingId))
                {
                    return StatusCode(400, "You have already applied to the Job");
                }
                var createdApplication = await _applicationServices.CreateApplicationAsync(model);
                return Ok(new
                {
                    success = true,
                    data = createdApplication
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while creating Application."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpPut]
        [Route("UpdateApplication")]
        public async Task<ActionResult<bool>> UpdateApplicationAsync([FromBody] Application model)
        {
            try
            {
                var application = await _applicationServices.UpdateApplicationAsync(model);
                if (application == false)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Application not found with given Id: {model.ApplicationId} "
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Application updated successfully"
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while updating Application."
                });
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpDelete]
        [Route("DeleteApplication/{id}")]
        public async Task<ActionResult<bool>> DeleteApplicationAsync(string applicationId)
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while deleting Application."
                });
            }
        }
    }
}
