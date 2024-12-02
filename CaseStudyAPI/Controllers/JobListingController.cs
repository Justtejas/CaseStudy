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
    public class JobListingController : ControllerBase
    {
        private readonly IJobListingServices _jobListingServices;
        private readonly ILogger<JobListingController> _logger;
        public JobListingController(IJobListingServices jobListingServices, ILogger<JobListingController> logger)
        {
            _jobListingServices = jobListingServices;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllJobListings")]
        public async Task<IActionResult> GetAllJobListingsAsync()
        {
            try
            {
                var jobListings = await _jobListingServices.GetAllJobListingsAsync();
                if (jobListings == null || jobListings.Count <= 0)
                {
                    return NotFound(new ApiResponse<List<JobListing>> { Success = false, Message = "No data found" });
                }
                return Ok(new ApiResponse<List<JobListing>> { Success = true, Data = jobListings });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while fetching Job Listings." });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetJobListingsById/{jobListingId}")]
        public async Task<IActionResult> GetJobListingsByIdAsync(string jobListingId)
        {
            try
            {
                var jobListing = await _jobListingServices.GetJobListingByIdAsync(jobListingId);

                if (jobListing == null)
                {
                    return NotFound(new ApiResponse<string> { Success = false, Message = "The Job Listing does not exist." });
                }
                return Ok(new ApiResponse<JobListing> { Success = true, Data = jobListing });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while fetching Job Listing." });
            }
        }
        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetJobListingsByEmployerId/{employerId}")]
        public async Task<IActionResult> GetJobListingByEmployerIdAsync(string employerId)
        {
            try
            {
                var jobListings = await _jobListingServices.GetJobListingByEmployerIdAsync(employerId);

                if (jobListings.Count == 0)
                {
                    return NotFound(new ApiResponse<string> { Success = false, Message = $"The Job Listing does not exist." });
                }
                return Ok(new ApiResponse<List<JobListing>> { Success = true, Data = jobListings });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while fetching Job Listing." });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        [Route("CreateJobListing")]
        public async Task<IActionResult> CreateJobListingAsync([FromBody] JobListingDTO jobListingData)
        {
            if (jobListingData == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(employerId))
                {
                    return Unauthorized(new ApiResponse<string> { Success = false, Error = "Employer is not authorized." });
                }

                var createdListing = await _jobListingServices.CreateJobListingAsync(jobListingData,employerId);
                if (createdListing == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while creating the job listing." });
                }

                return Ok(new ApiResponse<JobListing> { Success = true, Data = createdListing });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = ex.Message });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpPut]
        [Route("UpdateJobListing/{jobListingId}")]
        public async Task<IActionResult> UpdateJobListingAsync(string jobListingId, [FromBody] JobListingDTO model)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(jobListingId))
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Job Listing ID is required." });
                }

                if (model == null)
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid Job Listing data provided." });
                }

                var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(employerId))
                {
                    return Unauthorized(new ApiResponse<string> { Success = false, Message = "Unauthorized access. Employer ID not found." });
                }

                var jobListing = await _jobListingServices.UpdateJobListingAsync(jobListingId,employerId,model);
                return Ok(new ApiResponse<string> { Success = true, Message = "Job Listing updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Validation or business logic error occurred while updating job listing.");
                return BadRequest(new ApiResponse<string> { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while updating Job Listing." });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        [Route("GetJobListingByAvailablity/{vacancy}")]
        public async Task<IActionResult> GetJobListingByAvailability(bool vacancy)
        {
            try
            {
               var availableJobListings = await _jobListingServices.GetJobListingByAvailability(vacancy);
                if (availableJobListings == null || !availableJobListings.Any())
                {
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No job listings found for the specified availability." });
                }
                return Ok(new ApiResponse<List<JobListing>> { Success = true, Data = availableJobListings });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while fetching Job Listings." });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpDelete]
        [Route("DeleteJobListing/{jobListingId}")]
        public async Task<IActionResult> DeleteJobListingAsync(string jobListingId)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(jobListingId))
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Job Listing ID is required." });
                }
                var employerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(employerID))
                {
                    return Unauthorized(new ApiResponse<string> { Success = false, Message = "Unauthorized access. Employer ID not found." });
                }
                var resultMessage = await _jobListingServices.DeleteJobListingAsync(jobListingId, employerID);
                if (resultMessage.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new ApiResponse<string> { Success = true, Message = resultMessage });
                }
                else if (resultMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                         resultMessage.Contains("permission", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Message = resultMessage });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = resultMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting job listing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string> { Success = false, Error = "An error occurred while deleting Job Listing." });
            }
        }
    }
}
