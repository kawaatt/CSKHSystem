using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    /// <inheritdoc />
    public partial class updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramAccount",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIndividualWorking = table.Column<bool>(type: "bit", nullable: false),
                    ChatID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URLHooking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BotType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccount", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TelegramCustomers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TelegramID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    iPremium = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BotID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BotAffiliateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramCustomers", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramAccount");

            migrationBuilder.DropTable(
                name: "TelegramCustomers");
        }
    }
}
