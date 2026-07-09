using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// Awards, nominations & votes — AwardNominations: optional CreditFilmId + nullable FestivalFilmId.

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditNominees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AwardNominations_FestivalFilms_FestivalFilmId",
                table: "AwardNominations");

            migrationBuilder.AlterColumn<Guid>(
                name: "FestivalFilmId",
                table: "AwardNominations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldCollation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditFilmId",
                table: "AwardNominations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AwardNominations_CreditFilmId",
                table: "AwardNominations",
                column: "CreditFilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_AwardNominations_Credits_CreditFilmId",
                table: "AwardNominations",
                column: "CreditFilmId",
                principalTable: "Credits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AwardNominations_FestivalFilms_FestivalFilmId",
                table: "AwardNominations",
                column: "FestivalFilmId",
                principalTable: "FestivalFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AwardNominations_Credits_CreditFilmId",
                table: "AwardNominations");

            migrationBuilder.DropForeignKey(
                name: "FK_AwardNominations_FestivalFilms_FestivalFilmId",
                table: "AwardNominations");

            migrationBuilder.DropIndex(
                name: "IX_AwardNominations_CreditFilmId",
                table: "AwardNominations");

            migrationBuilder.DropColumn(
                name: "CreditFilmId",
                table: "AwardNominations");

            migrationBuilder.AlterColumn<Guid>(
                name: "FestivalFilmId",
                table: "AwardNominations",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldCollation: "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AwardNominations_FestivalFilms_FestivalFilmId",
                table: "AwardNominations",
                column: "FestivalFilmId",
                principalTable: "FestivalFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
