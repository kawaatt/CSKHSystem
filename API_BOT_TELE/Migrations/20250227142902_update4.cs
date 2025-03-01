using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    /// <inheritdoc />
    public partial class update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URLKeyboard",
                table: "TelegramResponse");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "TelegramResponse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "TelegramResponse");

            migrationBuilder.AddColumn<string>(
                name: "URLKeyboard",
                table: "TelegramResponse",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
