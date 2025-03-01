using API.Models.CSKHAuto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketRequest> TicketRequests { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }

        public DbSet<BOAccount> CheckAccountFilters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data for TicketCategory
            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "MÃ VÉ CƯỢC", IsActive = true, System = "F168" },
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "ĐƠN NẠP TIỀN", IsActive = true, System = "F168" },
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "ĐƠN RÚT TIỀN", IsActive = true, System = "F168" },
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "SỬA THÔNG TIN", IsActive = true, System = "F168" },
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "KHUYẾN MÃI", IsActive = true, System = "F168" },
                new TicketCategory { ID = Guid.NewGuid(), CategoryName = "VẤN ĐỀ KHÁC", IsActive = true, System = "F168" }
            );

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