using CaseStudyAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class JobListingDTO
    {
        [Required(ErrorMessage = "Job title is required.")]
        [MinLength(3, ErrorMessage = "Job title must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Job description is required.")]
        [MinLength(10, ErrorMessage = "Job description must be at least 10 characters long.")]
        public string JobDescription { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [MinLength(3, ErrorMessage = "Company name must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Hiring workflow is required.")]
        [MinLength(5, ErrorMessage = "Hiring workflow must be at least 5 characters long.")]
        public string HiringWorkflow { get; set; }

        [Required(ErrorMessage = "Eligibility criteria is required.")]
        [MinLength(5, ErrorMessage = "Eligibility criteria must be at least 5 characters long.")]
        public string EligibilityCriteria { get; set; }

        [Required(ErrorMessage = "Required skills are required.")]
        [MinLength(3, ErrorMessage = "Required skills must be at least 3 characters long.")]
        public string RequiredSkills { get; set; }

        [Required(ErrorMessage = "About the company is required.")]
        [MinLength(10, ErrorMessage = "About the company section must be at least 10 characters long.")]
        public string AboutCompany { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [MinLength(3, ErrorMessage = "Location must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Deadline is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for deadline.")]
        [FutureDate(ErrorMessage = "Deadline must be a future date.")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Vacancy status is required.")]
        public bool VacancyOfJob { get; set; }
    }
}
