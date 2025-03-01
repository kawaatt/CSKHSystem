using System.ComponentModel.DataAnnotations;

namespace ADMIN.Models.BotTele
{
    public class TelegramResponseDTO
    {
        public Guid ID { get; set; }
        public Guid BotID { get; set; }
        public string RequestCode { get; set; }
        public string? Content { get; set; }
        public string? URLImage { get; set; }
        public string? InlineKeyboard { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
