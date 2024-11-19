using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class ApplicationDTO
    {
        [Required(ErrorMessage = "Job Listing ID is required.")]
        public string JobListingId { get; set; }
    }
}
