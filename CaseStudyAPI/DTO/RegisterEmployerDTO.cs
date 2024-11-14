using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.DTO
{
    public class RegisterEmployerDTO
    {
        [Required]
        public string EmployerName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Username must be longer than 3 characters")]
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
        public string CompanyName { get; set; }
        [Required]
        public string ContactPhone { get; set; }
    }
}
