using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedOurApiRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "OurApiOffers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OurApiUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<string>(type: "text", nullable: true),
                    DocTypeId = table.Column<int>(type: "integer", nullable: true),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    JobTypeId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Income_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Income_CurrencyCode = table.Column<string>(type: "varchar(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OurApiUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OurApiOffers_QuoteId",
                table: "OurApiOffers",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_OurApiOffers_UserId",
                table: "OurApiOffers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OurApiOffers_OurApiQuotes_QuoteId",
                table: "OurApiOffers",
                column: "QuoteId",
                principalTable: "OurApiQuotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OurApiOffers_OurApiUsers_UserId",
                table: "OurApiOffers",
                column: "UserId",
                principalTable: "OurApiUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OurApiOffers_OurApiQuotes_QuoteId",
                table: "OurApiOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_OurApiOffers_OurApiUsers_UserId",
                table: "OurApiOffers");

            migrationBuilder.DropTable(
                name: "OurApiUsers");

            migrationBuilder.DropIndex(
                name: "IX_OurApiOffers_QuoteId",
                table: "OurApiOffers");

            migrationBuilder.DropIndex(
                name: "IX_OurApiOffers_UserId",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OurApiOffers");
        }
    }
}
