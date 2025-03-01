using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADMIN.Models.CSKHAuto
{
    //public class TicketHistory
    //{
    //    public Guid ID { get; set; }                // ID lịch sử

    //    public string UpdatedByUser { get; set; }           // ID người thay đổi

    //    public DateTime? UpdateTime { get; set; }     // Thời gian thay đổi trạng thái

    //    public string? TicketStatusTitle { get; set; }

    //    public string? TicketStatusDescription { get; set; }

    //    public int TicketStatusValue { get; set; }

    //    public string System { get; set; }

    //    public Guid TicketRequestID { get; set; } // Liên kết với TicketRequest

    //    public TicketRequest? TicketRequest { get; set; } // Liên kết với TicketRequest
    //}

    public class TicketHistoryDTO
    {
        public Guid TicketRequestID { get; set; }
        public string? UpdatedByUser { get; set; }           // ID người thay đổi

        public DateTime? UpdateTime { get; set; }     // Thời gian thay đổi trạng thái

        public string? TicketStatusTitle { get; set; }

        public string? TicketStatusDescription { get; set; }
        public int? TicketStatusValue { get; set; }
        public string? System { get; set; }
    }
}
