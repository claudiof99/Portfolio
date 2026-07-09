using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <summary>
    /// Replaces <c>ExternalFilmIds</c> JSON on PersonalLists with FK <c>FilmId</c> to Films and a unique index on (UserId, Type, FilmId).
    /// </summary>
    public partial class PersonalListUseFilmId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalFilmIds",
                table: "PersonalLists");

            migrationBuilder.AddColumn<Guid>(
                name: "FilmId",
                table: "PersonalLists",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalLists_FilmId",
                table: "PersonalLists",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalLists_UserId_Type_FilmId",
                table: "PersonalLists",
                columns: new[] { "UserId", "Type", "FilmId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalLists_Films_FilmId",
                table: "PersonalLists",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalLists_Films_FilmId",
                table: "PersonalLists");

            migrationBuilder.DropIndex(
                name: "IX_PersonalLists_FilmId",
                table: "PersonalLists");

            migrationBuilder.DropIndex(
                name: "IX_PersonalLists_UserId_Type_FilmId",
                table: "PersonalLists");

            migrationBuilder.DropColumn(
                name: "FilmId",
                table: "PersonalLists");

            migrationBuilder.AddColumn<string>(
                name: "ExternalFilmIds",
                table: "PersonalLists",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
