using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
	/// <summary>Creates the in-app <c>Notifications</c> queue (final schema: no ActionUrl / dismiss-only flags).</summary>
	public partial class AddNotification : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Notifications",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
					CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
					Title = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Message = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					CorrelationId = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					TargetUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
					TargetUserRole = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "Customer")
						.Annotation("MySql:CharSet", "utf8mb4"),
					AcknowledgedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Notifications", x => x.Id);
					table.ForeignKey(
						name: "FK_Notifications_Users_TargetUserId",
						column: x => x.TargetUserId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				})
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateIndex(
				name: "IX_Notifications_CreatedUtc",
				table: "Notifications",
				column: "CreatedUtc");

			migrationBuilder.CreateIndex(
				name: "IX_Notifications_TargetUserId_AcknowledgedUtc",
				table: "Notifications",
				columns: ["TargetUserId", "AcknowledgedUtc"]);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Notifications");
		}
	}
}
