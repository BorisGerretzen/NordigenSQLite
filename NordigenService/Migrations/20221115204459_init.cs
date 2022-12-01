using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NordigenService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    EndToEndId = table.Column<string>(type: "TEXT", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreditorName = table.Column<string>(type: "TEXT", nullable: true),
                    CreditorIban = table.Column<string>(type: "TEXT", nullable: true),
                    CreditorBban = table.Column<string>(type: "TEXT", nullable: true),
                    DebtorName = table.Column<string>(type: "TEXT", nullable: true),
                    DebtorIban = table.Column<string>(type: "TEXT", nullable: true),
                    DebtorBban = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    RemittanceInformationUnstructured = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
