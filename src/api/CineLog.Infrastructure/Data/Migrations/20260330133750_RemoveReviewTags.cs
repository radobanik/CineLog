using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReviewTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tags",
                table: "reviews",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
