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
                jobListing.PostedDate = DateTime.Now;
                var jobListingCreated = await _appDBContext.JobListings.AddAsync(jobListing);
                await _appDBContext.SaveChangesAsync();
                return jobListingCreated.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteJobListingAsync(string jobListingId, string employerId)
        {
            try
            {
                var jobListing = await _appDBContext.JobListings.FirstOrDefaultAsync(j => j.JobListingId == jobListingId && j.EmployerId == employerId) ?? throw new InvalidOperationException("Job Listing not found or you do not have permission to delete it."); ;
                if (jobListing == null)
                {
                    return false;
                }

                _appDBContext.JobListings.Remove(jobListing);
                await _appDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<List<JobListing>> GetAllJobListingsAsync()
        {
            return await _appDBContext.JobListings.Include(e => e.Employer).Include(a => a.Applications).ToListAsync();
        }

        public async Task<List<JobListing>> GetJobListingByAvailability(bool vacancy)
        {
            var availbleJobListings = await _appDBContext.JobListings.Include(e => e.Employer).Include(a => a.Applications).Where(j => j.VacancyOfJob == vacancy).ToListAsync();
            return availbleJobListings;
        }

        public async Task<List<JobListing>> GetJobListingByEmployerIdAsync(string employerId)
        {
            return await _appDBContext.JobListings.Include(e => e.Employer).Include(a => a.Applications)
           .Where(j => j.EmployerId == employerId).ToListAsync();
        }

        public async Task<JobListing> GetJobListingByIdAsync(string jobListingId)
        {
            var jobListing = await _appDBContext.JobListings.Include(e => e.Employer).Include(a => a.Applications).FirstOrDefaultAsync(j => j.JobListingId == jobListingId);
            if (jobListing == null)
            {
                return null;
            }
            return jobListing;
        }

        public async Task<bool> UpdateJobListingAsync(string jobListingId, string employerId, JobListingDTO jobListingDTO)
        {
            try
            {
                var existingJobListing = await _appDBContext.JobListings
                    .FirstOrDefaultAsync(j => j.JobListingId ==
                    jobListingId && j.EmployerId == employerId) ?? throw new
                    InvalidOperationException("Job Listing not found or you do not have permission to update it.");;

                if (existingJobListing == null)
                {
                    return false;
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
