namespace SHBET_CLIENT.Models.DTO
{
    public class TicketRequestDTO
    {
        public Guid ID { get; set; }

        public string Account { get; set; }

        public string TicketContent { get; set; }

        public string System { get; set; }

        public string? ImageURL { get; set; }

        public string? ImageBase64 { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? CardHolder { get; set; }

        public Guid TicketCategoryID { get; set; }

        public TicketCategoryDTO? TicketCategory { get; set; }

        public ICollection<TicketHistoryDTO>? TicketHistories { get; set; }
    }

    public class TicketHistoryDTO
    {
        public string? UpdatedByUser { get; set; }           // ID người thay đổi

        public DateTime? UpdateTime { get; set; }     // Thời gian thay đổi trạng thái

        public string? TicketStatusTitle { get; set; }

        public string? TicketStatusDescription { get; set; }
        public int TicketStatusValue { get; set; }

    }

    public class TicketCategoryDTO
    {
        public Guid ID { get; set; }          // ID loại vấn đề
        public string CategoryName { get; set; }     // Tên loại vấn đề
    }
}
