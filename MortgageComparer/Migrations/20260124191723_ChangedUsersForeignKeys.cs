using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUsersForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
