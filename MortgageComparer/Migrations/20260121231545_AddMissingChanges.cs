using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Offers_QuoteId",
                table: "Offers",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Quotes_QuoteId",
                table: "Offers",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Quotes_QuoteId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_QuoteId",
                table: "Offers");
        }
    }
}
