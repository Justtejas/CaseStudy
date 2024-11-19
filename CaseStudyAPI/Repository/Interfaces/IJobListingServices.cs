﻿using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IJobListingServices
    {
        public Task<List<JobListing>> GetAllJobListingsAsync();
        public Task<JobListing> GetJobListingByIdAsync(string jobListingId);
        public Task<List<JobListing>> GetJobListingByAvailability(bool vacancy);
        public Task<List<JobListing>> GetJobListingByEmployerIdAsync(string employerId);
        public Task<JobListing> CreateJobListingAsync(JobListingDTO jobListingDTO, string employerId);
        public Task<bool> UpdateJobListingAsync(JobListing jobListing);
        public Task<bool> DeleteJobListingAsync(string jobListingId,string employerId);
    }
}
