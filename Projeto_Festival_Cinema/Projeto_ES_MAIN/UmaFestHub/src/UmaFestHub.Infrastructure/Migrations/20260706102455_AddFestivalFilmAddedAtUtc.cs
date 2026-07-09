using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFestivalFilmAddedAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAtUtc",
                table: "FestivalFilms",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "FestivalFilms",
                keyColumn: "Id",
                keyValue: new Guid("8d690e52-e89c-4b9d-b994-7f4e67d9e323"),
                column: "AddedAtUtc",
                value: new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.Sql(
                """
                UPDATE FestivalFilms ff
                INNER JOIN Films f ON ff.FilmId = f.Id
                SET ff.AddedAtUtc = f.CreatedAtUtc
                WHERE ff.AddedAtUtc = '2026-01-01 00:00:00'
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedAtUtc",
                table: "FestivalFilms");
        }
    }
}
