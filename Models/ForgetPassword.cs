using System.ComponentModel.DataAnnotations;

namespace szpont.Models

{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "{0} jest wymagane."), EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
