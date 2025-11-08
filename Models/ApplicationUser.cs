using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace szpont.Models
{
    [Index(nameof(StudentIndex), IsUnique =true)]
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Role { get; set; } = "User";
        [Required, MaxLength(50)]
        public string StudentIndex { get; set; } = string.Empty;

        
    }
}