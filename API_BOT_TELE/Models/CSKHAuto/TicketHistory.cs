using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TELEBOT_CSKH.Models.CSKHAuto
{
    public class TicketHistory
    {
        [Key]
        public Guid ID { get; set; }                // ID lịch sử

        [Required]
        public string UpdatedByUser { get; set; }           // ID người thay đổi

        public DateTime? UpdateTime { get; set; }     // Thời gian thay đổi trạng thái

        [MaxLength(100)]
        public string? TicketStatusTitle { get; set; }

        [MaxLength(500)]
        public string? TicketStatusDescription { get; set; }

        [Required]
        public int TicketStatusValue { get; set; }

        [Required]
        public string System { get; set; }

        [Required]
        public Guid TicketRequestID { get; set; } // Liên kết với TicketRequest
        // Khai báo ForeignKey cho mối quan hệ với TicketRequest
        [ForeignKey("TicketRequestID")]
        public TicketRequest? TicketRequest { get; set; } // Liên kết với TicketRequest
    }

    public class TicketHistoryDTO
    {
        public string UpdatedByUser { get; set; }           // ID người thay đổi

        public DateTime? UpdateTime { get; set; }     // Thời gian thay đổi trạng thái

        public string? TicketStatusTitle { get; set; }

        public string? TicketStatusDescription { get; set; }
        public int TicketStatusValue { get; set; }

    }
}
