using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedQuoteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstalmentNumber",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "RequestedMonths",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "StatusDescription",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Quotes");

            migrationBuilder.RenameColumn(
                name: "TotalAmountToPay_CurrencyCode",
                table: "Quotes",
                newName: "InstalmentAmount_CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "TotalAmountToPay_Amount",
                table: "Quotes",
                newName: "InstalmentAmount_Amount");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Quotes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Quotes");

            migrationBuilder.RenameColumn(
                name: "InstalmentAmount_CurrencyCode",
                table: "Quotes",
                newName: "TotalAmountToPay_CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "InstalmentAmount_Amount",
                table: "Quotes",
                newName: "TotalAmountToPay_Amount");

            migrationBuilder.AddColumn<int>(
                name: "InstalmentNumber",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedMonths",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusDescription",
                table: "Quotes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
