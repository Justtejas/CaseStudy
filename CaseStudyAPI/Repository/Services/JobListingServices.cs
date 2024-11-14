using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class JobListingServices : IJobListingServices
    {

        private readonly ApplicationDBContext _appDBContext;
        public JobListingServices(ApplicationDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }
        public async Task<JobListing> CreateJobListingAsync(JobListing jobListing)
        {
            try
            {
                jobListing.JobListingId = Guid.NewGuid().ToString();
                jobListing.PostedDate = DateTime.Now;

                await _appDBContext.JobListings.AddAsync(jobListing);
                await _appDBContext.SaveChangesAsync();

                return jobListing;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                return false;
            }
        }

        public async Task<List<JobListing>> GetAllJobListingsAsync()
        {
            return await _appDBContext.JobListings.ToListAsync();
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

        public async Task<bool> UpdateJobListingAsync(JobListing jobListing)
        {
            try
            {
                var existingJobListing = await _appDBContext.JobListings
                    .FirstOrDefaultAsync(j => j.JobListingId == jobListing.JobListingId && j.EmployerId == jobListing.EmployerId) ??  throw new InvalidOperationException("Job Listing not found or you do not have permission to update it.");;

                if (existingJobListing == null)
                {
                    return false;
                }

                existingJobListing.JobTitle = jobListing.JobTitle;
                existingJobListing.JobDescription = jobListing.JobDescription;
                existingJobListing.CompanyName = jobListing.CompanyName;
                existingJobListing.HiringWorkflow = jobListing.HiringWorkflow;
                existingJobListing.EligibilityCriteria = jobListing.EligibilityCriteria;
                existingJobListing.RequiredSkills = jobListing.RequiredSkills;
                existingJobListing.AboutCompany = jobListing.AboutCompany;
                existingJobListing.Location = jobListing.Location;
                existingJobListing.Salary = jobListing.Salary;
                existingJobListing.PostedDate = jobListing.PostedDate;
                existingJobListing.Deadline = jobListing.Deadline;
                existingJobListing.VacancyOfJob = jobListing.VacancyOfJob;

                await _appDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
