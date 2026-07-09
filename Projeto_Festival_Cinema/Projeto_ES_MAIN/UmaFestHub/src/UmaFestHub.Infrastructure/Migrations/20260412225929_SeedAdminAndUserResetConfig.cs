using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndUserResetConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAtUtc", "Email", "Name", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "PasswordSalt", "Roles" },
                values: new object[] { new Guid("3a737c6a-a355-40fd-936c-159d5c680ac2"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@umafesthub.com", "Admin", "7xyCsrH7kxwRBDbtNQllLgoHQAh7GKXI6FnpootOj+Q=", null, null, "2Ona5c7q3G4VY8i1QmN6XA==", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3a737c6a-a355-40fd-936c-159d5c680ac2"));

            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
