using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UmaFestHub.Infrastructure.Data;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
	[DbContext(typeof(AppDbContext))]
	[Migration("20260602134000_AddAwardEndDateUtc")]
	public partial class AddAwardEndDateUtc : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "EndDateUtc",
				table: "Awards",
				type: "datetime(6)",
				nullable: true);

			migrationBuilder.Sql(
				"UPDATE `Awards` SET `EndDateUtc` = DATE_ADD(`CreatedAtUtc`, INTERVAL 30 DAY) WHERE `EndDateUtc` IS NULL");

			migrationBuilder.AlterColumn<DateTime>(
				name: "EndDateUtc",
				table: "Awards",
				type: "datetime(6)",
				nullable: false,
				oldClrType: typeof(DateTime),
				oldType: "datetime(6)",
				oldNullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "EndDateUtc",
				table: "Awards");
		}
	}
}
