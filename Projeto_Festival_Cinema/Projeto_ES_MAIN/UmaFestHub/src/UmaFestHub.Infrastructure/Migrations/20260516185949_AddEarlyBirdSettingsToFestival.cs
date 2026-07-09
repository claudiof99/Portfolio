using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEarlyBirdSettingsToFestival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarlyBirdDaysBeforeStart",
                table: "Festivals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EarlyBirdDiscountPercent",
                table: "Festivals",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Festivals",
                keyColumn: "Id",
                keyValue: new Guid("d96b6b25-2b87-4e10-8f50-6d194ab49022"),
                columns: new[] { "EarlyBirdDaysBeforeStart", "EarlyBirdDiscountPercent" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyBirdDaysBeforeStart",
                table: "Festivals");

            migrationBuilder.DropColumn(
                name: "EarlyBirdDiscountPercent",
                table: "Festivals");
        }
    }
}
