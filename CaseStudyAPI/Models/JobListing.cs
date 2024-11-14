namespace CaseStudyAPI.Models
{
    public class JobListing
    {
        public JobListing()
        {
            Applications = new List<Application>();
        }
        public string JobListingId { get; set; }
        public string EmployerId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public string CompanyName { get; set; }
        public string HiringWorkflow { get; set; }
        public string EligibilityCriteria { get; set; }
        public string RequiredSkills { get; set; }
        public string AboutCompany { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime Deadline { get; set; }
        public bool? VacancyOfJob { get; set; }

        public virtual Employer? Employer { get; set; }
        public virtual List<Application>? Applications { get; set; }
    }
}
