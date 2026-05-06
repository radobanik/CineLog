using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchlistTypeAndUserBio : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
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

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
