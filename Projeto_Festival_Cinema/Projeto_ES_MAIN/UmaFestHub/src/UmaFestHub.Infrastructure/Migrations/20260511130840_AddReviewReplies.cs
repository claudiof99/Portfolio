using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UmaFestHub.Infrastructure.Data;

#nullable disable

// Schema: ReviewReplies table (initial columns; moderation added in AddReviewReplyModeration).

namespace UmaFestHub.Infrastructure.Migrations;

/// <summary>Adds <c>ReviewReplies</c> for threaded responses to film reviews.</summary>
[DbContext(typeof(AppDbContext))]
[Migration("20260511130840_AddReviewReplies")]
public class AddReviewReplies : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "ReviewReplies",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
				ReviewId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
				UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
				Comment = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false)
					.Annotation("MySql:CharSet", "utf8mb4"),
				DateUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_ReviewReplies", x => x.Id);
				table.ForeignKey(
					name: "FK_ReviewReplies_Reviews_ReviewId",
					column: x => x.ReviewId,
					principalTable: "Reviews",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				table.ForeignKey(
					name: "FK_ReviewReplies_Users_UserId",
					column: x => x.UserId,
					principalTable: "Users",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			})
			.Annotation("MySql:CharSet", "utf8mb4");

		migrationBuilder.CreateIndex(
			name: "IX_ReviewReplies_ReviewId",
			table: "ReviewReplies",
			column: "ReviewId");

		migrationBuilder.CreateIndex(
			name: "IX_ReviewReplies_UserId",
			table: "ReviewReplies",
			column: "UserId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(name: "ReviewReplies");
	}
}
