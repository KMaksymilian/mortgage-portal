using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class AddedContractToDatbase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contract",
                table: "Offers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ContractData",
                table: "Offers",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contract",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ContractData",
                table: "Offers");
        }
    }
}
