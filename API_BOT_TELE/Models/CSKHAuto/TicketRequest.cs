using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace TELEBOT_CSKH.Models.CSKHAuto
{
    public class TicketRequest
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string TicketCode { get; set; }

        public DateTime? RequestDate { get; set; }

        [MaxLength(500)] // Giới hạn độ dài mô tả yêu cầu
        public string? TicketContent { get; set; }

        [Required]
        public string System { get; set; }

        [Required]
        [ForeignKey("TicketCategory")]
        public Guid TicketCategoryID { get; set; }
        public TicketCategory? TicketCategory { get; set; }


        public ICollection<TicketHistory>? TicketHistories { get; set; }
    }

    public class TicketRequestDTO
    {
        public Guid ID { get; set; }

        public string Account { get; set; }

        public string TicketCode { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? TicketContent { get; set; }

        public string System { get; set; }

        public string? CardHolder { get; set; }

        public TicketCategoryDTO? TicketCategory { get; set; }

        public ICollection<TicketHistoryDTO>? TicketHistories { get; set; }
    }
}
