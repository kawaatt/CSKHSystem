using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    /// <inheritdoc />
    public partial class updatedb9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAccount",
                table: "TelegramCampaign",
                newName: "TotalAccountPending");

            migrationBuilder.AddColumn<int>(
                name: "TotalAccountFinish",
                table: "TelegramCampaign",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAccountFinish",
                table: "TelegramCampaign");

            migrationBuilder.RenameColumn(
                name: "TotalAccountPending",
                table: "TelegramCampaign",
                newName: "TotalAccount");
        }
    }
}
