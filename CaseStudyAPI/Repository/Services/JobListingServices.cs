using AutoMapper;
using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class JobListingServices : IJobListingServices
    {

        private readonly ApplicationDBContext _appDBContext;
        private readonly IMapper _mapper;
        private readonly ILogger<JobListingServices> _logger;
        public JobListingServices(ApplicationDBContext appDBContext, IMapper mapper, ILogger<JobListingServices> logger)
        {
            _appDBContext = appDBContext;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<JobListing> CreateJobListingAsync(JobListingDTO jobListingDTO, string employerId)
        {
            try
            {
                var jobListing = _mapper.Map<JobListing>(jobListingDTO);
                jobListing.JobListingId = Guid.NewGuid().ToString();
                jobListing.EmployerId = employerId;
                jobListing.PostedDate = DateTime.UtcNow;

                var jobListingCreated = await _appDBContext.JobListings.AddAsync(jobListing);
                await _appDBContext.SaveChangesAsync();

                _logger.LogInformation($"Job listing with ID {jobListing.JobListingId} created successfully for employer {employerId}.");

                return jobListingCreated.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the job listing.");
                return null;
            }
        }

        public async Task<string> DeleteJobListingAsync(string jobListingId, string employerId)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(jobListingId))
                {
                    throw new ArgumentNullException(nameof(jobListingId), "Job Listing ID cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(employerId))
                {
                    throw new ArgumentNullException(nameof(employerId), "Employer ID cannot be null or empty.");
                }
                var jobListing = await _appDBContext.JobListings
                    .FirstOrDefaultAsync(j => j.JobListingId == jobListingId && j.EmployerId == employerId);

                if (jobListing == null)
                {
                    return "Job Listing not found or you do not have permission to delete it.";
                }

                _appDBContext.JobListings.Remove(jobListing);
                await _appDBContext.SaveChangesAsync();

                _logger.LogInformation($"Job listing with ID {jobListingId} successfully deleted for employer {employerId}. Job Title: {jobListing.JobTitle}");
                return $"Deleted job listing successfully for job title {jobListing.JobTitle}";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Job Listing deletion failed.");
                return ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the job listing.");
                return "An error occurred while deleting the job listing.";
            }
        }

        public async Task<List<JobListing>> GetAllJobListingsAsync()
        {
            return await _appDBContext.JobListings.ToListAsync();
        }

        public async Task<List<JobListing>> GetJobListingByAvailability(bool vacancy)
        {
            var availbleJobListings = await _appDBContext.JobListings.Where(j => j.VacancyOfJob == vacancy).ToListAsync();
            return availbleJobListings;
        }

        public async Task<List<JobListing>> GetJobListingByEmployerIdAsync(string employerId)
        {
            return await _appDBContext.JobListings
           .Where(j => j.EmployerId == employerId).ToListAsync();
        }

        public async Task<JobListing> GetJobListingByIdAsync(string jobListingId)
        {
            var jobListing = await _appDBContext.JobListings.FirstOrDefaultAsync(j => j.JobListingId == jobListingId);
            if (jobListing == null)
            {
                return null;
            }
            return jobListing;
        }

        public async Task<string> UpdateJobListingAsync(string jobListingId, string employerId, JobListingDTO jobListingDTO)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(jobListingId))
                {
                    throw new ArgumentNullException(nameof(jobListingId), "Job Listing ID cannot be null or empty.");
                }
                if (string.IsNullOrWhiteSpace(employerId))
                {
                    throw new ArgumentNullException(nameof(employerId), "Employer ID cannot be null or empty.");
                }
                if (jobListingDTO == null)
                {
                    throw new ArgumentNullException(nameof(jobListingDTO), "Job Listing data cannot be null.");
                }
                var existingJobListing = await _appDBContext.JobListings
                    .FirstOrDefaultAsync(j => j.JobListingId ==
                    jobListingId && j.EmployerId == employerId);

                if (existingJobListing == null)
                {
                    throw new InvalidOperationException("Job Listing not found or you do not have permission to update it.");
                }
                existingJobListing.JobTitle = jobListingDTO.JobTitle;
                existingJobListing.JobDescription = jobListingDTO.JobDescription;
                existingJobListing.CompanyName = jobListingDTO.CompanyName;
                existingJobListing.HiringWorkflow = jobListingDTO.HiringWorkflow;
                existingJobListing.EligibilityCriteria = jobListingDTO.EligibilityCriteria;
                existingJobListing.RequiredSkills = jobListingDTO.RequiredSkills;
                existingJobListing.AboutCompany = jobListingDTO.AboutCompany;
                existingJobListing.Location = jobListingDTO.Location;
                existingJobListing.Salary = jobListingDTO.Salary;
                existingJobListing.Deadline = jobListingDTO.Deadline;
                existingJobListing.VacancyOfJob = jobListingDTO.VacancyOfJob;

                await _appDBContext.SaveChangesAsync();
                _logger.LogInformation($"Job Listing {jobListingId} updated successfully for employer {employerId}.");
                return "Updated Job Listing Successfully";
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Invalid input data.");
                return ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Job listing not found or permission issue.");
                return ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the job listing.");
                return "An error occurred while updating the job listing.";
            }
        }
    }
}
