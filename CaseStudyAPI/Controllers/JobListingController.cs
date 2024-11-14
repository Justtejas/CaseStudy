using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobListingController : ControllerBase
    {
        private readonly IJobListingServices _jobListingServices;
        public JobListingController(IJobListingServices jobListingServices)
        {
            _jobListingServices = jobListingServices;
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetAllJobListings")]
        public async Task<ActionResult<List<JobListing>>> GetAllJobListingsAsync()
        {
            try
            {
                var jobListings = await _jobListingServices.GetAllJobListingsAsync();
                if (jobListings == null || jobListings.Count <= 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No data found"
                    });
                }
                return Ok(jobListings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Job Listings."
                });
            }
        }

        [Authorize(Roles = "Employer,JobSeeker")]
        [HttpGet]
        [Route("GetJobListingsById/{jobListingId}")]
        public async Task<ActionResult<JobListing>> GetJobListingsByIdAsync(string jobListingId)
        {
            try
            {
                var jobListing = await _jobListingServices.GetJobListingByIdAsync(jobListingId);

                if (jobListing == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'Listing' with Id: {jobListingId} not found"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = jobListing
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching Listing."
                });
            }
        }
        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetJobListingsByEmployerId/{employerId}")]
        public async Task<ActionResult<JobListing>> GetJobListingByEmployerIdAsync(string employerId)
        {
            try
            {
                var jobListings = await _jobListingServices.GetJobListingByEmployerIdAsync(employerId);

                if (jobListings.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"The 'JobListings' with Id: {employerId} not found"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = jobListings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while fetching JobListings."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        [Route("CreateJobListing")]
        public async Task<ActionResult<JobListing>> CreateJobListingAsync([FromBody] JobListing jobListing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                jobListing.EmployerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdListing = await _jobListingServices.CreateJobListingAsync(jobListing);

                if (createdListing == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        success = false,
                        error = "An error occurred while saving the job listing."
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = createdListing
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpPut]
        [Route("UpdateJobListing")]
        public async Task<ActionResult<bool>> UpdateJobListingAsync([FromBody] JobListing model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid Data"
                    });
                }
                model.EmployerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var jobListing = await _jobListingServices.UpdateJobListingAsync(model);

                if (jobListing == false)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Job Listing not found with given Id: {model.JobListingId} "
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Job Listing updated successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while updating Job Listing."
                });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpDelete]
        [Route("DeleteJobListing/{jobListingId}")]
        public async Task<ActionResult<bool>> DeleteJobListingAsync(string jobListingId)
        {
            try
            {
                var employerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var jobListing = await _jobListingServices.DeleteJobListingAsync(jobListingId, employerID);
                if (jobListing == false)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid Job Listing Id"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Job Listing deleted successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = "An error occurred while deleting Job Listing."
                });
            }
        }
    }
}
