using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseStudyAPI.Models
{
    public class JobSeeker
    {
        public JobSeeker()
        {
            Applications = new List<Application>();
            Resumes = new List<Resume>();
        }
        public string JobSeekerId { get; set; }
        public string JobSeekerName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public string Institute { get; set; }
        public int Year { get; set; }
        public decimal CGPA { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Role { get; set; }
        [JsonIgnore]
        public virtual List<Application> Applications { get; set; }
        [JsonIgnore]
        public virtual List<Resume> Resumes { get; set; }
    }
}
