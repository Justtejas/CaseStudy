namespace CaseStudyAPI.Models
{
    public class Application
    {
        public string ApplicationId { get; set; }
        public string JobListingId { get; set; }
        public string JobSeekerId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; }
        public virtual JobListing? JobListing { get; set; }
        public virtual JobSeeker? JobSeeker { get; set; }
    }
}
