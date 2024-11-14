using CaseStudyAPI.Data;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IAuthorizationService
    {
        public Task<TokenResponse> GenerateJWTTokenAsync<T>(T user);
        public Task<string> HashPasswordAsync(string password);
        public Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
    }
}
