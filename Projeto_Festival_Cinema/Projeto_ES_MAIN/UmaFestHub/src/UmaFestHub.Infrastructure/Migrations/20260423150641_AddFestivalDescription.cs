using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFestivalDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Festivals");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Festivals",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Festivals",
                keyColumn: "Id",
                keyValue: new Guid("d96b6b25-2b87-4e10-8f50-6d194ab49022"),
                column: "Description",
                value: "Join us for a week of independent and international films celebrating diverse storytelling.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Festivals");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Festivals",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Festivals",
                keyColumn: "Id",
                keyValue: new Guid("d96b6b25-2b87-4e10-8f50-6d194ab49022"),
                column: "Location",
                value: "Mumbai");
        }
    }
}
