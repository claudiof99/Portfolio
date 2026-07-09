using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

// Awards, nominations & votes — schema: Vote composite key (UserId + AwardNominationId).

namespace UmaFestHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixVoteCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
