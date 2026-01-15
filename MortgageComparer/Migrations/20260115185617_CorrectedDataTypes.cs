using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedDataTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "OurApiQuotes");

            migrationBuilder.AddColumn<int>(
                name: "AmountToPay",
                table: "OurApiQuotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedAmount",
                table: "OurApiQuotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OurApiOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuoteId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", nullable: false),
                    MonthlyInstallementAmount = table.Column<int>(type: "integer", nullable: false),
                    MonthlyInstallementCurrency = table.Column<string>(type: "text", nullable: false),
                    RequestedAmount = table.Column<int>(type: "integer", nullable: false),
                    RequestedCurrency = table.Column<string>(type: "text", nullable: false),
                    RequestedPeriodInMonths = table.Column<int>(type: "integer", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiOffers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "AmountToPay",
                table: "OurApiQuotes");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "OurApiQuotes");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "OurApiQuotes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
