using CaseStudyAPI.Data;

namespace CaseStudyAPI.Repository.Interfaces
{
    public interface IAuthorizationService
    {
        public TokenResponse GenerateJWTToken<T>(T user);
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hashedPassword);
    }
}
