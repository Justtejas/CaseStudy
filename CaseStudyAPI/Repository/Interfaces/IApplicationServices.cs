using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IApplicationServices
    {
        public Task<List<Application>> GetAllApplicationsAsync();
        public Task<Application> GetApplicationByIdAsync(string applicationId);
        public Task<List<Application>> GetApplicationByJSIdAsync(string jobSeekerId);
        public Task<List<Application>> GetApplicationByJobListingIdAsync(string jobListingId);
        public Task<List<Application>> GetApplicationByEmployerIDAsync(string employerId);
        public Task<Application> CreateApplicationAsync(ApplicationDTO applicationDTO,string jobSeekerId);
        public Task<bool> UpdateApplicationAsync(string applicationId, string applicationStatus);
        public Task<bool> DeleteApplicationAsync(string applicationId, string jobSeekerId);
    }
}
