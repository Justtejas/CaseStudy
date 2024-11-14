using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository
{
    public interface IJobListingServices
    {
        public Task<List<JobListing>> GetAllJobListingsAsync();
        public Task<JobListing> GetJobListingByIdAsync(string jobListingId);
        public Task<List<JobListing>> GetJobListingByEmployerIdAsync(string employerId);
        public Task<JobListing> CreateJobListingAsync(JobListing jobListing);
        public Task<bool> UpdateJobListingAsync(JobListing jobListing);
        public Task<bool> DeleteJobListingAsync(string jobListingId);
    }
}
