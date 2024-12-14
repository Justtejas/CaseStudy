using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IJobSeekerServices
    {
        public Task<JobSeeker> GetJobSeekerByUserNameAsync(string userName);
        public Task<JobSeeker> GetJobSeekerByJobSeekerIdAsync(string jobSeekerId);
        public Task<List<JobSeeker>> GetAllJobSeekersAsync();
        public Task<Response> CreateJobSeekerAsync(JobSeeker jobseekers);
        public Task<Response> DeleteJobSeekerAsync(string id);
        public Task<Response> UpdateJobSeekerAsync(string id, UpdateJobSeekerDTO jobSeekerModel);
    }
}
