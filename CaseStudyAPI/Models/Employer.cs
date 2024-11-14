using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.Models
{
    public class Employer
    {
         public Employer()
        {
            JobListings = new List<JobListing>();
        }
        public string EmployerId { get; set; }
        public string EmployerName { get; set; }
        [MinLength(3, ErrorMessage = "Username should be longer than 3 characters")]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string CompanyName { get; set; }
        public string ContactPhone { get; set; } 
        public string? Role { get; set; }
        public virtual List<JobListing> JobListings { get; set; }
    }
}
