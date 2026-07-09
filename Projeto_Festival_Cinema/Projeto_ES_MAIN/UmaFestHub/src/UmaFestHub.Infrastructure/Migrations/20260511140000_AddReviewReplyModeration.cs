using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UmaFestHub.Infrastructure.Data;

#nullable disable

// Schema: moderation columns on ReviewReplies (aligns with Reviews moderation shape).

namespace UmaFestHub.Infrastructure.Migrations;

/// <summary>Adds moderation columns to <c>ReviewReplies</c> (same shape as <c>Reviews</c>).</summary>
[DbContext(typeof(AppDbContext))]
[Migration("20260511140000_AddReviewReplyModeration")]
public class AddReviewReplyModeration : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<bool>(
			name: "HasBeenReported",
			table: "ReviewReplies",
			type: "tinyint(1)",
			nullable: false,
			defaultValue: false);

		migrationBuilder.AddColumn<bool>(
			name: "IsHiddenByAdmin",
			table: "ReviewReplies",
			type: "tinyint(1)",
			nullable: false,
			defaultValue: false);

		migrationBuilder.AddColumn<bool>(
			name: "IsReported",
			table: "ReviewReplies",
			type: "tinyint(1)",
			nullable: false,
			defaultValue: false);

		migrationBuilder.AddColumn<string>(
			name: "Status",
			table: "ReviewReplies",
			type: "varchar(30)",
			maxLength: 30,
			nullable: false,
			defaultValue: "Approved")
			.Annotation("MySql:CharSet", "utf8mb4");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(name: "HasBeenReported", table: "ReviewReplies");
		migrationBuilder.DropColumn(name: "IsHiddenByAdmin", table: "ReviewReplies");
		migrationBuilder.DropColumn(name: "IsReported", table: "ReviewReplies");
		migrationBuilder.DropColumn(name: "Status", table: "ReviewReplies");
	}
}
