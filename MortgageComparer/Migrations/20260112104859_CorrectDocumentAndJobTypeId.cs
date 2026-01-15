using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDocumentAndJobTypeId : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "JobTypes",
                newName: "JobTypeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DocumentTypes",
                newName: "PersonalDocumentId");
        }
    }
}
