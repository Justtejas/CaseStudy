using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseStudyAPI.Models
{
    public class Application
    {
        [JsonIgnore]
        public string ApplicationId { get; set; }
        [Required]
        public string JobListingId { get; set; }
        [JsonIgnore]
        public string JobSeekerId { get; set; }
        public DateTime ApplicationDate { get; set; }
        [Required]
        public string ApplicationStatus { get; set; }
        [JsonIgnore]
        public virtual JobListing? JobListing { get; set; }
        [JsonIgnore]
        public virtual JobSeeker? JobSeeker { get; set; }
    }
}
