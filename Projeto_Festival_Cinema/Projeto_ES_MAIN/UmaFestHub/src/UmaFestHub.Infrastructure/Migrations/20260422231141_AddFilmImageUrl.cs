using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilmImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Films",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Films",
                keyColumn: "Id",
                keyValue: new Guid("6e75788b-0a9c-4602-a5de-e53a1f6d3a01"),
                column: "ImageUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Films");
        }
    }
}
