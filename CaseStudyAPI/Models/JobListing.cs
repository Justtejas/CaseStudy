using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseStudyAPI.Models
{
    public class JobListing
    {
        public JobListing()
        {
            Applications = new List<Application>();
        }
        [JsonIgnore]
        public string JobListingId { get; set; }
        [JsonIgnore]
        public string EmployerId { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string JobDescription { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string HiringWorkflow { get; set; }
        [Required]
        public string EligibilityCriteria { get; set; }
        [Required]
        public string RequiredSkills { get; set; }
        [Required]
        public string AboutCompany { get; set; }
        [Required]
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime Deadline { get; set; }
        public bool? VacancyOfJob { get; set; }

        [JsonIgnore]
        public virtual Employer? Employer { get; set; }
        [JsonIgnore]
        public virtual List<Application> Applications { get; set; }
    }
}
