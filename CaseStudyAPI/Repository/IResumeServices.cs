using CaseStudyAPI.Data;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository
{
    public interface IResumeServices
    {
        public Task<Resume> GetResumeAsync(string resumeId, string jobSeekerId);
        public Task<bool> DeleteResumeAsync(string resumeId, string jobSeekerId);
        public Task<string> UpdateResumeAsync(string resumeId, string jobSeekerId, IFormFile newFile);
        public Task<Response> CreateResumeAsync(string jobSeekerId,IFormFile resume);
    }
}
