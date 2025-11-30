using System.ComponentModel.DataAnnotations;

namespace szpont.Models

{
    public class ForgetPassword
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
