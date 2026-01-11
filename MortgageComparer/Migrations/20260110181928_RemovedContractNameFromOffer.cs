using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedContractNameFromOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contract",
                table: "Offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contract",
                table: "Offers",
                type: "text",
                nullable: true);
        }
    }
}
