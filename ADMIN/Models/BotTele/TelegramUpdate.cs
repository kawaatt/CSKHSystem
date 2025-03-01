using System.Text.Json.Serialization;

namespace ADMIN.Models.BotTele
{
    public class TelegramUpdate
    {
        public long update_id { get; set; }

        public TelegramMessage? message { get; set; }

        public CallbackQuery? callback_query { get; set; }
    }

    public class TelegramMessage
    {
        public int message_id { get; set; }

        public TelegramUser from { get; set; }

        public TelegramChat chat { get; set; }

        public long date { get; set; }

        public string text { get; set; }
    }

    public class TelegramUser
    {
        public long id { get; set; }

        public bool is_bot { get; set; }

        public string first_name { get; set; }

        public string username { get; set; }

        public string language_code { get; set; }

        public bool is_premium { get; set; }
    }

    public class TelegramChat
    {
        public long id { get; set; }

        public string first_name { get; set; }

        public string username { get; set; }

        public string type { get; set; }
    }

    public class CallbackQuery
    {
        public string? id { get; set; }

        public TelegramUser? from { get; set; }

        public string? data { get; set; }
    }
}
