using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class UpdateEmployerDTO
    {
        [Required]
        public string EmployerName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Username must be longer than 3 characters")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^Male$|^Female$", ErrorMessage = "Gender must be Male or Female.")]
        public string Gender { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Company name must be longer than 2 characters.")]
        public string CompanyName { get; set; }
        [Required]
        [RegularExpression(@"^\+?[1-9]\d{10,12}$", ErrorMessage = "Invalid contact phone number.")]
        public string ContactPhone { get; set; }
    }
}
