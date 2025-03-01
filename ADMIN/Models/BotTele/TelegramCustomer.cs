using System.ComponentModel.DataAnnotations;

namespace ADMIN.Models.BotTele
{
    public class TelegramCustomer
    {
        [Key]
        public Guid ID { get; set; }
        public string TelegramID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public bool iPremium { get; set; }
        public DateTime? CreateDate { get; set; }
        public string System { get; set; }
    }

    public class TelegramCustomerDTO
    {
        public Guid ID { get; set; }
        public string TelegramID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public bool iPremium { get; set; }
        public DateTime? CreateDate { get; set; }
        public string System { get; set; }
    }
}
