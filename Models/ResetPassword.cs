using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
namespace szpont.Models
{
    public class ResetPassword
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must be identical.")]
        public string ConfirmPassword { get; set; }
    }
}
