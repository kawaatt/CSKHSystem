using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;

namespace TELEBOT_CSKH.Models.TELEGRAM_BOT
{
    public class TelegramUpdate
    {
        public long update_id { get; set; }

        public TelegramMessage? message { get; set; }

        public CallbackQuery? callback_query { get; set; }
    }

    public class TelegramMessage
    {
        public long message_id { get; set; }

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

        public string type { get; set; }
    }

    public class CallbackQuery
    {
        public string? id { get; set; }

        public TelegramUser? from { get; set; }

        public string? data { get; set; }
    }

    public class UserFinancialInfoDTO
    {
        public string Currency { get; set; }
        public string Username { get; set; }
        public long UserIdx { get; set; }
        public long ParentId { get; set; }
        public string ParentName { get; set; }
        public string MemberLevelName { get; set; }
        public string VipLevelName { get; set; }
        public string ChannelName { get; set; }
        public int DepositTimes { get; set; }
        public int DepositDays { get; set; }
        public string Deposit { get; set; }
        public int WithdrawTimes { get; set; }
        public string Withdraw { get; set; }
        public string DepositWithdrawDiff { get; set; }
        public int BetTimes { get; set; }
        public string TotalBet { get; set; }
        public string ValidBet { get; set; }
        public string ProfitLose { get; set; }
        public string Commission { get; set; }
        public string Fund { get; set; }
        public string DiscountAmount { get; set; }
        public string ActivityReward { get; set; }
        public string TaskReward { get; set; }
        public string RebateReward { get; set; }
        public string VipAmount { get; set; }
        public string RechargeDeduce { get; set; }
        public string YuebaoProfit { get; set; }
        public string ArtificialDeposit { get; set; }
        public string ArtificialWithdraw { get; set; }
        public string GiveUpDiscount { get; set; }
        public string BlindBox { get; set; }
        public string Credit { get; set; }
        public string Repay { get; set; }
    }

    public class CheckAgentResultDTO
    {
        public int NewRegister { get; set; }
        public int NewCustomer { get; set; }
        public int DepositCustomer { get; set; }
        public int TotalDeposit { get; set; }
        public int TotalWithdraw { get; set; }
        public int TotalOnline { get; set; }
        public int TotalValidBet { get; set; }
        public int TurnAround { get; set; }
    }
}
