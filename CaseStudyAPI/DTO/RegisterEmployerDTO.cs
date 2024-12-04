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
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Confirm Password must match Password.")]
        public string ConfirmPassword { get; set; }
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
