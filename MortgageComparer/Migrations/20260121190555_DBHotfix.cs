using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class DBHotfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobTypeId",
                table: "JobTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PersonalDocumentId",
                table: "DocumentTypes",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Income",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IncomeCurrCode",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JobEndDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "JobStartDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "BankPercentage",
                table: "Offers",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ContractData",
                table: "Offers",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OurApiQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestedAmount = table.Column<int>(type: "integer", nullable: false),
                    AmountToPay = table.Column<int>(type: "integer", nullable: false),
                    Installments = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiQuotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OurApiUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<string>(type: "text", nullable: true),
                    DocTypeId = table.Column<int>(type: "integer", nullable: true),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    JobTypeId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Income_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Income_CurrencyCode = table.Column<string>(type: "varchar(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuoteId = table.Column<int>(type: "integer", nullable: false),
                    RequestedMonths = table.Column<int>(type: "integer", nullable: false),
                    TotalAmountToPay_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmountToPay_CurrencyCode = table.Column<string>(type: "varchar(3)", nullable: true),
                    InstalmentNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    StatusDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

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
                    DocumentKey = table.Column<string>(type: "text", nullable: false),
                    Contract = table.Column<byte[]>(type: "bytea", nullable: true),
                    SignedContract = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OurApiOffers_OurApiQuotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "OurApiQuotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OurApiOffers_OurApiUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "OurApiUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OurApiOffers");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "OurApiQuotes");

            migrationBuilder.DropTable(
                name: "OurApiUsers");

            migrationBuilder.DropColumn(
                name: "Income",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IncomeCurrCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobStartDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContractData",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "JobTypes",
                newName: "JobTypeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DocumentTypes",
                newName: "PersonalDocumentId");

            migrationBuilder.AlterColumn<int>(
                name: "BankPercentage",
                table: "Offers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
