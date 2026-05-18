using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
namespace szpont.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "{0} jest wymagane."), EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Token jest wymagany.")]
        public string Token { get; set; }
        [Required(ErrorMessage = "{0} jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        [Required(ErrorMessage = "{0} jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        [Display(Name = "Powtórz hasło")]
        public string ConfirmPassword { get; set; }
    }
}
