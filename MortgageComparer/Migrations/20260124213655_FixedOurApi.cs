using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class FixedOurApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DocumentTypes_PersonalDocumentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PersonalDocumentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonalDocumentId",
                table: "Users");

            migrationBuilder.AlterColumn<decimal>(
                name: "InstalmentAmount_Amount",
                table: "Quotes",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "Quotes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentId",
                table: "Users",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DocumentTypes_DocumentId",
                table: "Users",
                column: "DocumentId",
                principalTable: "DocumentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DocumentTypes_DocumentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DocumentId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "PersonalDocumentId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InstalmentAmount_Amount",
                table: "Quotes",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "Quotes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonalDocumentId",
                table: "Users",
                column: "PersonalDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DocumentTypes_PersonalDocumentId",
                table: "Users",
                column: "PersonalDocumentId",
                principalTable: "DocumentTypes",
                principalColumn: "Id");
        }
    }
}
