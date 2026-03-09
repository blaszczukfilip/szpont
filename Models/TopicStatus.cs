using System.ComponentModel.DataAnnotations;

namespace szpont.Models
{
    public enum TopicStatus
    {
        [Display(Name = "Szkic")]
        Draft,
        [Display(Name = "Oczekuje na Kierownika")]
        WaitingForKierownik,
        [Display(Name = "Oczekuje na Dziekana")]
        WaitingForDziekan,
        [Display(Name = "Zatwierdzony")]
        Approved,
        [Display(Name = "Odrzucony")]
        Rejected
    }
}