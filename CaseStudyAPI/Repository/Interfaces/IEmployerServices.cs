using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IEmployerServices
    {
        public Task<Employer> GetEmployerByEmployerIdAsync(string employerId);
        public Task<Employer> GetEmployerByUserName(string userName);
        public Task<List<Employer>> GetAllEmployersAsync();
        public Task<Response> CreateEmployerAsync(Employer employer);
        public Task<Response> DeleteEmployerAsync(string employerId);
        public Task<Response> UpdateEmployerAsync(string employerId, UpdateEmployerDTO employer);
    }
}
