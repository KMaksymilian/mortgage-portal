using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedQuoteParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalQuoteId",
                table: "Quotes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Months",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalQuoteId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Months",
                table: "Quotes");
        }
    }
}
