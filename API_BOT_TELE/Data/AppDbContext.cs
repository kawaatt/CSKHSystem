using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TELEBOT_CSKH.Models.CSKHAuto;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;

namespace TELEBOT_CSKH.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<TelegramAccount> TelegramAccount { get; set; }
        public DbSet<TelegramCustomer> TelegramCustomers { get; set; }

        public DbSet<TelegramResponse> TelegramResponse { get; set; }
        public DbSet<TelegramCampaign> TelegramCampaign { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }
    }
}