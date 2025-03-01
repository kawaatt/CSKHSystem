using System.ComponentModel.DataAnnotations;

namespace TELEBOT_CSKH.Models.CSKHAuto
{
    public class TicketCategory
    {
        [Key]
        public Guid ID { get; set; }          // ID loại vấn đề
        [Required]
        public string CategoryName { get; set; }     // Tên loại vấn đề
    }

    public class TicketCategoryDTO
    {
        public Guid ID { get; set; }          // ID loại vấn đề
        public string CategoryName { get; set; }     // Tên loại vấn đề
    }
}
