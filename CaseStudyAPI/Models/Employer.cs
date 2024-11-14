using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseStudyAPI.Models
{
    public class Employer
    {
         public Employer()
        {
            JobListings = new List<JobListing>();
        }
        [JsonIgnore]
        public string EmployerId { get; set; }
        [Required]
        public string EmployerName { get; set; }
        [MinLength(3, ErrorMessage = "Username should be longer than 3 characters")]
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [JsonIgnore]
        public string? Role { get; set; }
        [JsonIgnore]
        public virtual List<JobListing> JobListings { get; set; }
    }
}
