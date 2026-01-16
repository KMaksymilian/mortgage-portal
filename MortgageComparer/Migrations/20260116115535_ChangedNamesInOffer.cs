using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamesInOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignedDocument",
                table: "OurApiOffers",
                newName: "SignedContract");

            migrationBuilder.RenameColumn(
                name: "Document",
                table: "OurApiOffers",
                newName: "Contract");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignedContract",
                table: "OurApiOffers",
                newName: "SignedDocument");

            migrationBuilder.RenameColumn(
                name: "Contract",
                table: "OurApiOffers",
                newName: "Document");
        }
    }
}
