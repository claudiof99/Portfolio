using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// Awards, nominations & votes — schema: Votes no longer store a numeric Value (choice is the row).

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVoteValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Votes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
