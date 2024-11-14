using CaseStudyAPI.Data;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository
{
    public interface IEmployerServices
    {
        public Task<Employer> GetEmployerByUserName(string UserName);
        public Task<Response> CreateEmployerAsync(Employer employer);
        public Task<Response> DeleteEmployerAsync(string id);
    }
}
