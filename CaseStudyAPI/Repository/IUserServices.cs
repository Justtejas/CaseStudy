using CaseStudyAPI.Authentication;

namespace CaseStudyAPI.Repository
{
    public interface IUserServices
    {
        public Task<TokenResponse?> Login(LoginModel model);
        public Task<Response> Register(RegisterModel model);
    }
}
