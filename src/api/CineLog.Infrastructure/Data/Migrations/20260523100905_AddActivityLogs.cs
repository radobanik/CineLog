using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: true),
                    WatchlistId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_logs_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_activity_logs_reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activity_logs_watchlists_WatchlistId",
                        column: x => x.WatchlistId,
                        principalTable: "watchlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_ActorUserId",
                table: "activity_logs",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_CreatedAt",
                table: "activity_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_MovieId",
                table: "activity_logs",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_ReviewId",
                table: "activity_logs",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_TargetUserId",
                table: "activity_logs",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_WatchlistId",
                table: "activity_logs",
                column: "WatchlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");
        }
    }
}
