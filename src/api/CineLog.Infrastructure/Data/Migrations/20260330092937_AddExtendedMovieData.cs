using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedMovieData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Budget",
                table: "movies",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "movies",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsManuallyEdited",
                table: "movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfEpisodes",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSeasons",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalLanguage",
                table: "movies",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalTitle",
                table: "movies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Popularity",
                table: "movies",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "movies",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "movies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "movies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfilePath = table.Column<string>(type: "text", nullable: true),
                    Biography = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Popularity = table.Column<double>(type: "double precision", nullable: true),
                    SyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "production_companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoPath = table.Column<string>(type: "text", nullable: true),
                    OriginCountry = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movie_cast",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Character = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreditId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_cast", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movie_cast_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_cast_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movie_crew",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Job = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreditId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_crew", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movie_crew_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_crew_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movie_production_companies",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_production_companies", x => new { x.MovieId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_movie_production_companies_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_production_companies_production_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "production_companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_MovieId_PersonId_CreditId",
                table: "movie_cast",
                columns: new[] { "MovieId", "PersonId", "CreditId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_PersonId",
                table: "movie_cast",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_MovieId_PersonId_CreditId",
                table: "movie_crew",
                columns: new[] { "MovieId", "PersonId", "CreditId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_PersonId",
                table: "movie_crew",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_production_companies_CompanyId",
                table: "movie_production_companies",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movie_cast");

            migrationBuilder.DropTable(
                name: "movie_crew");

            migrationBuilder.DropTable(
                name: "movie_production_companies");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "production_companies");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "IsManuallyEdited",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "NumberOfEpisodes",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "NumberOfSeasons",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "OriginalLanguage",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "OriginalTitle",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "movies");
        }
    }
}
