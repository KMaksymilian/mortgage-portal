using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageComparer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserAndOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Income",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IncomeCurrCode",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JobEndDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "JobStartDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Income",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IncomeCurrCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobStartDate",
                table: "Users");
        }
    }
}
