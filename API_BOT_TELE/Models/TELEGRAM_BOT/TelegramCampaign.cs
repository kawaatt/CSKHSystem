using System.ComponentModel.DataAnnotations;

namespace TELEBOT_CSKH.Models.TELEGRAM_BOT
{
    public class TelegramCampaign
    {
        [Key]
        public Guid ID { get; set; }
        public Guid IDBot { get; set; }
        public string? ImageURL { get; set; }
        public string? Content{ get; set; }
        public string? InlineKeyboard { get; set; }
        public int TotalAccountPending { get; set; } = 0;
        public int TotalAccountFinish { get; set; } = 0;
        public bool iRun { get; set; } = false;
    }

    public class TelegramCampaignDTO
    {
        public Guid ID { get; set; }
        public Guid IDBot { get; set; }
        public string? ImageURL { get; set; }
        public string? Content { get; set; }
        public string? InlineKeyboard { get; set; }
        public int TotalAccountPending { get; set; } = 0;
        public int TotalAccountFinish { get; set; } = 0;
        public bool iRun { get; set; } = false;
    }
}
