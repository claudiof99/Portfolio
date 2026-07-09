using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Persons",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Persons");
        }
    }
}
