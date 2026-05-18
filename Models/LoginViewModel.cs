using System.ComponentModel.DataAnnotations;

namespace szpont.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} jest wymagane."), EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; } = false;
    }
}