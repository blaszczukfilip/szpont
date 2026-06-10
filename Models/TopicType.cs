using System.ComponentModel.DataAnnotations;

namespace szpont.Models
{
    public enum TopicType
    {
        [Display(Name = "Licencjacka")]
        Licencjacka,

        [Display(Name = "Inżynierska")]
        Inzynierska,

        [Display(Name = "Magisterska")]
        Magisterska,

        [Display(Name = "Doktorska")]
        Doktorska
    }
}
