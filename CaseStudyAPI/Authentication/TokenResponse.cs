namespace CaseStudyAPI.Authentication
{
    public class TokenResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
