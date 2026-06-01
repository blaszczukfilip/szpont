using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace szpont.Models
{
    public enum ReservationStatus
    {
        [Display(Name = "Oczekiwanie na akceptację przez promotora")]
        Pending,

        [Display(Name = "Zaakceptowana przez promotora")]
        Accepted,

        [Display(Name = "Odrzucona przez promotora")]
        Rejected
    }

    public static class ReservationStatusHelper
    {
        public static string GetDisplayName(ReservationStatus? status)
        {
            if (status is null)
                return "—";

            var field = typeof(ReservationStatus).GetField(status.Value.ToString());
            var display = field?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? status.Value.ToString();
        }

        public static ReservationStatus? EffectiveStatus(string? studentId, ReservationStatus? status)
        {
            if (string.IsNullOrEmpty(studentId))
                return null;
            return status ?? ReservationStatus.Accepted;
        }

        public static bool HasBeenAccepted(Topic topic)
        {
            if (topic == null || string.IsNullOrEmpty(topic.StudentId))
                return false;

            return EffectiveStatus(topic.StudentId, topic.ReservationStatus) == ReservationStatus.Accepted;
        }

        public const string CannotModifyWithAcceptedReservationMessage =
            "Nie można edytować/usuwać tematu z zaakceptowaną rezerwacją.";
    }
}
