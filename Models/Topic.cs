using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace szpont.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TopicType Type { get; set; }

        [MaxLength(200)]
        public string Keywords { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public TopicStatus Status { get; set; } = TopicStatus.Draft;

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [Required]
        public string PromotorId { get; set; } = string.Empty;
        
        public string? KierownikId { get; set; }
        
        public string? DziekanId { get; set; }

        [ForeignKey("PromotorId")]
        public virtual ApplicationUser? Promotor { get; set; }

        [ForeignKey("KierownikId")]
        public virtual ApplicationUser? Kierownik { get; set; }

        [ForeignKey("DziekanId")]
        public virtual ApplicationUser? Dziekan { get; set; }

        public string? StudentId { get; set; }

        public DateTime? ReservationDate { get; set; }

        public ReservationStatus? ReservationStatus { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser? Student { get; set; }
    }
}
