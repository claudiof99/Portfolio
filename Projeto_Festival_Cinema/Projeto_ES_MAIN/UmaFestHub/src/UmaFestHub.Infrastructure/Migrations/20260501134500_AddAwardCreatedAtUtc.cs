// Awards, nominations & votes — schema: Awards.CreatedAtUtc (ordering/audit).
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Migrations
{
	[DbContext(typeof(AppDbContext))]
	[Migration("20260501134500_AddAwardCreatedAtUtc")]
	public partial class AddAwardCreatedAtUtc : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAtUtc",
				table: "Awards",
				type: "datetime(6)",
				nullable: false,
				defaultValueSql: "CURRENT_TIMESTAMP(6)");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CreatedAtUtc",
				table: "Awards");
		}
	}
}

