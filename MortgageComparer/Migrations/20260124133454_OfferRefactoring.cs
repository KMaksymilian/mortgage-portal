using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class OfferRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OurApiOffers_OurApiQuotes_QuoteId",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "OurApiOffers");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "OurApiOffers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Offers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Offers",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<byte[]>(
                name: "ContractData",
                table: "OurApiOffers",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractLinkValidDate",
                table: "OurApiOffers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentLink",
                table: "OurApiOffers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SingedBy",
                table: "OurApiOffers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OurApiOffers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusDescription",
                table: "OurApiOffers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OurApiOffers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusDescription",
                table: "Offers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentLink",
                table: "Offers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OurApiOffers_Quotes_QuoteId",
                table: "OurApiOffers",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OurApiOffers_Quotes_QuoteId",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "ContractData",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "ContractLinkValidDate",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "DocumentLink",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "SingedBy",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "StatusDescription",
                table: "OurApiOffers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OurApiOffers");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OurApiOffers",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Offers",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Offers",
                newName: "CreateDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "OurApiOffers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "StatusDescription",
                table: "Offers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentLink",
                table: "Offers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OurApiOffers_OurApiQuotes_QuoteId",
                table: "OurApiOffers",
                column: "QuoteId",
                principalTable: "OurApiQuotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
