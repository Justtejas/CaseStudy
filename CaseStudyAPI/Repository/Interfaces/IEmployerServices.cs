using CaseStudyAPI.Data;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IEmployerServices
    {
        public Task<Employer> GetEmployerByUserName(string UserName);
        public Task<List<Employer>> GetAllEmployersAsync();
        public Task<Response> CreateEmployerAsync(Employer employer);
        public Task<Response> DeleteEmployerAsync(string employerId);
        public Task<Response> UpdateEmployerAsync(string employerId, Employer employer);
    }
}
