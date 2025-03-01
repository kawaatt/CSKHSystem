using System.ComponentModel.DataAnnotations;

namespace API.Models.CSKHAuto
{
    public class TicketCategory
    {
        [Key]
        public Guid ID { get; set; }          // ID loại vấn đề
        [Required]
        public string CategoryName { get; set; }     // Tên loại vấn đề
        [Required]
        public string System {  get; set; }
        [Required]
        public bool? IsActive { get; set; }
    }

    public class TicketCategoryDTO
    {
        public Guid ID { get; set; }          // ID loại vấn đề
        public string CategoryName { get; set; }     // Tên loại vấn đề
    }
}
