using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class RegisterJobSeekerDTO
    {
        [Required]
        public string JobSeekerName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Username should be longer than 3 characters")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Institute { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public decimal CGPA { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
