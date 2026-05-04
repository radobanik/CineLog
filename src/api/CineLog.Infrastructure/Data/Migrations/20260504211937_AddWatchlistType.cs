using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchlistType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "watchlists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_watchlists_UserId_Type",
                table: "watchlists",
                columns: new[] { "UserId", "Type" },
                unique: true,
                filter: "\"Type\" <> 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_watchlists_UserId_Type",
                table: "watchlists");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "watchlists");
        }
    }
}
