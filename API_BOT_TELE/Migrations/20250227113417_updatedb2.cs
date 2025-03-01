using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    /// <inheritdoc />
    public partial class updatedb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "TelegramResponse",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BotID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URLImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URLKeyboard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InlineKeyboard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramResponse", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramKeyboard");

            migrationBuilder.DropTable(
                name: "TelegramResponse");
        }
    }
}
