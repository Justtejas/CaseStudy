using CaseStudyAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class UpdateJobSeekerDTO
    {
        [Required]
        public string JobSeekerName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Username should be longer than 3 characters")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^Male$|^Female$", ErrorMessage = "Gender must be Male or Female.")]
        public string Gender { get; set; }
        [Required]
        [RegularExpression(@"^\+?[1-9]\d{10,12}$", ErrorMessage = "Invalid contact phone number.")]
        public string ContactPhone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidator), nameof(DateValidator.ValidateDateOfBirth))]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Institute { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }
        [Required]
        [Range(0.0, 10.0, ErrorMessage = "CGPA must be between 0 and 10.")]
        public decimal CGPA { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidator), nameof(DateValidator.ValidateEndDate))]
        public DateTime EndDate { get; set; }
    }
}
