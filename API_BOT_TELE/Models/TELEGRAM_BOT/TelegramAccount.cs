﻿using System.ComponentModel.DataAnnotations;

namespace TELEBOT_CSKH.Models.TELEGRAM_BOT
{
    public class TelegramAccount
    {
        [Key]
        public Guid ID { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public bool IsIndividualWorking { get; set; }
        public string? ChatID { get; set; }
        public string? URLHooking { get; set; }
        public int BotType { get; set; }
        public bool Status { get; set; }
        public string? KeyboardData { get; set; }
        public string System { get; set; }
        public bool fRunCampaign { get; set; } = false;
    }

    public class TelegramAccountDTO
    {
        public Guid ID { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public bool IsIndividualWorking { get; set; }
        public string? ChatID { get; set; }
        public string? URLHooking { get; set; }
        public int BotType { get; set; }
        public bool Status { get; set; }
        public string? KeyboardData { get; set; }
        public string System { get; set; }
        public bool fRunCampaign { get; set; } = false;
    }
}
