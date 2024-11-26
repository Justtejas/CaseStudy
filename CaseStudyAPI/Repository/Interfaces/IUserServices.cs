using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IUserServices
    {
        public Task<Response> RegisterEmployerAsync(RegisterEmployerDTO regEmployer);
        public Task<Response> RegisterJobSeekerAsync(RegisterJobSeekerDTO regJobSeeker);
        public TokenResponse Login<T>(T login);
    }
}
