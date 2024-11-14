namespace CaseStudyAPI.Data
{
    public class TokenResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
