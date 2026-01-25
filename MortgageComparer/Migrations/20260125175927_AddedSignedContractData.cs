using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class AddedSignedContractData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "SignedContractData",
                table: "Offers",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignedFileContents",
                table: "Offers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignedFileName",
                table: "Offers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignedContractData",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "SignedFileContents",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "SignedFileName",
                table: "Offers");
        }
    }
}
