using CaseStudyAPI.Data;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IResumeServices
    {
        public Task<Resume> GetResumeAsync(string jobSeekerId);
        public Task<bool> DeleteResumeAsync(string resumeId, string jobSeekerId);
        public Task<Response> CreateResumeAsync(string jobSeekerId, IFormFile resume);
    }
}
