using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IApplicationServices
    {
        public Task<List<Application>> GetAllApplicationsAsync();
        public Task<Application> GetApplicationByIdAsync(string applicationId);
        public Task<List<Application>> GetApplicationByJSIdAsync(string jobSeekerId);
        public Task<List<Application>> GetApplicationByJobListingIdAsync(string jobListingId);
        public Task<IEnumerable<Application>> GetApplicationByEmployerIDAsync(string employerId);
        public Task<Application> CreateApplicationAsync(Application application);
        public Task<bool> UpdateApplicationAsync(Application application);
        public Task<bool> DeleteApplicationAsync(string applicationId, string jobSeekerId);
    }
}
