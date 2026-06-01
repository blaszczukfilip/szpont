using System.ComponentModel.DataAnnotations;

namespace szpont.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} jest wymagane."), StringLength(50, ErrorMessage = "{0} może mieć maksymalnie 50 znaków.")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), StringLength(50, ErrorMessage = "{0} może mieć maksymalnie 50 znaków.")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), StringLength(10, ErrorMessage = "{0} może mieć maksymalnie 10 znaków.")]
        [Display(Name = "Numer indeksu")]
        public string StudentIndex { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} jest wymagane."), DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        [Display(Name = "Powtórz hasło")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}