using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.Authentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; } = null!;
        [EmailAddress]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = null!;
    }
}
