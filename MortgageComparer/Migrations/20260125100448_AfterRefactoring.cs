using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class AfterRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OurApiOffers");

            migrationBuilder.DropTable(
                name: "OurApiQuotes");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "ExternalQuoteId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "BankPercentage",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ContractData",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ExternalBankOfferId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "MonthlyInstallment_Amount",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "MonthlyInstallment_CurrencyCode",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "RequestedMoney_Amount",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "RequestedMoney_CurrencyCode",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "InstalmentAmount_CurrencyCode",
                table: "Quotes",
                type: "varchar(3)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount_Amount",
                table: "Quotes",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RequestedAmount_CurrencyCode",
                table: "Quotes",
                type: "varchar(3)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Percentage",
                table: "Offers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "OfferToBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BankCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferToBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteToBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuoteId = table.Column<int>(type: "integer", nullable: false),
                    BankCode = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteToBanks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferToBanks");

            migrationBuilder.DropTable(
                name: "QuoteToBanks");

            migrationBuilder.DropColumn(
                name: "RequestedAmount_Amount",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "RequestedAmount_CurrencyCode",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "InstalmentAmount_CurrencyCode",
                table: "Quotes",
                type: "varchar(3)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Quotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExternalQuoteId",
                table: "Quotes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount",
                table: "Quotes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Offers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "BankPercentage",
                table: "Offers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ContractData",
                table: "Offers",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalBankOfferId",
                table: "Offers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyInstallment_Amount",
                table: "Offers",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonthlyInstallment_CurrencyCode",
                table: "Offers",
                type: "varchar(3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedMoney_Amount",
                table: "Offers",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedMoney_CurrencyCode",
                table: "Offers",
                type: "varchar(3)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OurApiOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuoteId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Contract = table.Column<byte[]>(type: "bytea", nullable: true),
                    ContractData = table.Column<byte[]>(type: "bytea", nullable: true),
                    ContractLinkValidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentKey = table.Column<string>(type: "text", nullable: false),
                    DocumentLink = table.Column<string>(type: "text", nullable: true),
                    MonthlyInstallementAmount = table.Column<int>(type: "integer", nullable: false),
                    MonthlyInstallementCurrency = table.Column<string>(type: "text", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", nullable: false),
                    RequestedAmount = table.Column<int>(type: "integer", nullable: false),
                    RequestedCurrency = table.Column<string>(type: "text", nullable: false),
                    RequestedPeriodInMonths = table.Column<int>(type: "integer", nullable: false),
                    SignedContract = table.Column<byte[]>(type: "bytea", nullable: true),
                    SingedBy = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusDescription = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OurApiOffers_OurApiUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "OurApiUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OurApiOffers_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OurApiQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountToPay = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Installments = table.Column<int>(type: "integer", nullable: false),
                    InstalmentRate = table.Column<int>(type: "integer", nullable: false),
                    RequestedAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiQuotes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OurApiOffers_QuoteId",
                table: "OurApiOffers",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_OurApiOffers_UserId",
                table: "OurApiOffers",
                column: "UserId");
        }
    }
}
