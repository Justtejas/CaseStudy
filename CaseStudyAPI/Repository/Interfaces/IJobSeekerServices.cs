using CaseStudyAPI.Data;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IJobSeekerServices
    {
        public Task<JobSeeker> GetJobSeekerByUserName(string userName);
        public Task<Response> CreateJobSeekerAsync(JobSeeker jobseekers);
        public Task<Response> DeleteJobSeekerAsync(string id);
    }
}
