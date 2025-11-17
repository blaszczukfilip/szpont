using System.ComponentModel.DataAnnotations;

namespace szpont.Models
{
    public class RegisterViewModel
    {
        [Required, MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        [Display(Name = "Student Index")]
        public string StudentIndex { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must be identical.")]
        [Display(Name = "Repeat password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}