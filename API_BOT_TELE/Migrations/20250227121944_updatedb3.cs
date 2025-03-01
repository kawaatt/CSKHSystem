using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    /// <inheritdoc />
    public partial class updatedb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramKeyboard");

            migrationBuilder.AddColumn<string>(
                name: "KeyboardData",
                table: "TelegramAccount",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyboardData",
                table: "TelegramAccount");

            migrationBuilder.CreateTable(
                name: "TelegramKeyboard",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BotID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Keyboard = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramKeyboard", x => x.ID);
                });
        }
    }
}
