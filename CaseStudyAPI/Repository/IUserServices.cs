using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;

namespace CaseStudyAPI.Repository
{
    public interface IUserServices
    {
        public Task<Response> RegisterEmployerAsync(RegisterEmployerDTO regEmployer);
        public Task<Response> RegisterJobSeekerAsync(RegisterJobSeekerDTO regJobSeeker);
        public Task<TokenResponse?> LoginAsync<T>(T login);
    }
}
