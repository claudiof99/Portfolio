// Awards, nominations & votes — schema: Awards.IsActive (toggle voting visibility).
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Migrations
{
	[DbContext(typeof(AppDbContext))]
	[Migration("20260501190000_AddAwardIsActive")]
	public partial class AddAwardIsActive : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "Awards",
				type: "tinyint(1)",
				nullable: false,
				defaultValue: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "Awards");
		}
	}
}
