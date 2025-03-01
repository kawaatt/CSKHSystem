using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckAccountFilters",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckAccountFilters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategories",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TicketRequests",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketContent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TicketCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TicketRequests_TicketCategories_TicketCategoryID",
                        column: x => x.TicketCategoryID,
                        principalTable: "TicketCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TicketStatusTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TicketStatusDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TicketStatusValue = table.Column<int>(type: "int", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TicketHistories_TicketRequests_TicketRequestID",
                        column: x => x.TicketRequestID,
                        principalTable: "TicketRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TicketCategories",
                columns: new[] { "ID", "CategoryName", "IsActive", "System" },
                values: new object[,]
                {
                    { new Guid("572b9965-52cc-4815-afb7-b3f589bb7dd5"), "KHUYẾN MÃI", true, "F168" },
                    { new Guid("663acba9-866e-4799-9bd5-c32c2d2705c2"), "ĐƠN NẠP TIỀN", true, "F168" },
                    { new Guid("750714d6-dcef-404f-b6c0-cc1ae9baf9d9"), "MÃ VÉ CƯỢC", true, "F168" },
                    { new Guid("8d31ec4e-7358-40fc-80eb-15713970f2c5"), "ĐƠN RÚT TIỀN", true, "F168" },
                    { new Guid("91a4d714-2729-4971-8b76-ac6220908e0b"), "SỬA THÔNG TIN", true, "F168" },
                    { new Guid("bb8d5cc2-598e-47ee-9793-ea48ffef1a3c"), "VẤN ĐỀ KHÁC", true, "F168" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_TicketRequestID",
                table: "TicketHistories",
                column: "TicketRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRequests_TicketCategoryID",
                table: "TicketRequests",
                column: "TicketCategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckAccountFilters");

            migrationBuilder.DropTable(
                name: "TicketHistories");

            migrationBuilder.DropTable(
                name: "TicketRequests");

            migrationBuilder.DropTable(
                name: "TicketCategories");
        }
    }
}
