using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CineLog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UseGuidPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename movies.TmdbId → IdTmdb (simple rename, no type change)
            migrationBuilder.RenameColumn(
                name: "TmdbId",
                table: "movies",
                newName: "IdTmdb");

            migrationBuilder.RenameIndex(
                name: "IX_movies_TmdbId",
                table: "movies",
                newName: "IX_movies_IdTmdb");

            // Drop dependent junction tables first (have FKs into persons/genres/production_companies)
            migrationBuilder.DropTable(name: "movie_cast");
            migrationBuilder.DropTable(name: "movie_crew");
            migrationBuilder.DropTable(name: "movie_genres");
            migrationBuilder.DropTable(name: "movie_production_companies");

            // Drop base tables whose PKs change from int to uuid
            migrationBuilder.DropTable(name: "persons");
            migrationBuilder.DropTable(name: "genres");
            migrationBuilder.DropTable(name: "production_companies");

            // Recreate persons with Guid PK + IdTmdb
            migrationBuilder.CreateTable(
                name: "persons",
                columns: t => new
                {
                    Id = t.Column<Guid>(type: "uuid", nullable: false),
                    IdTmdb = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfilePath = t.Column<string>(type: "text", nullable: true),
                    Biography = t.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Birthday = t.Column<DateOnly>(type: "date", nullable: true),
                    PlaceOfBirth = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Popularity = t.Column<double>(type: "double precision", nullable: true),
                    SyncedAt = t.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_persons", x => x.Id));

            migrationBuilder.CreateIndex(
                name: "IX_persons_IdTmdb",
                table: "persons",
                column: "IdTmdb",
                unique: true);

            // Recreate genres with Guid PK + IdTmdb
            migrationBuilder.CreateTable(
                name: "genres",
                columns: t => new
                {
                    Id = t.Column<Guid>(type: "uuid", nullable: false),
                    IdTmdb = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_genres", x => x.Id));

            migrationBuilder.CreateIndex(
                name: "IX_genres_IdTmdb",
                table: "genres",
                column: "IdTmdb",
                unique: true);

            // Recreate production_companies with Guid PK + IdTmdb
            migrationBuilder.CreateTable(
                name: "production_companies",
                columns: t => new
                {
                    Id = t.Column<Guid>(type: "uuid", nullable: false),
                    IdTmdb = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoPath = t.Column<string>(type: "text", nullable: true),
                    OriginCountry = t.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: t => t.PrimaryKey("PK_production_companies", x => x.Id));

            migrationBuilder.CreateIndex(
                name: "IX_production_companies_IdTmdb",
                table: "production_companies",
                column: "IdTmdb",
                unique: true);

            // Recreate movie_cast with Guid PersonId FK
            migrationBuilder.CreateTable(
                name: "movie_cast",
                columns: t => new
                {
                    Id = t.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = t.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = t.Column<Guid>(type: "uuid", nullable: false),
                    Character = t.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Order = t.Column<int>(type: "integer", nullable: false),
                    CreditId = t.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_movie_cast", x => x.Id);
                    t.ForeignKey("FK_movie_cast_movies_MovieId", x => x.MovieId, "movies", "Id", onDelete: ReferentialAction.Cascade);
                    t.ForeignKey("FK_movie_cast_persons_PersonId", x => x.PersonId, "persons", "Id", onDelete: ReferentialAction.Restrict);
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

            // Recreate movie_crew with Guid PersonId FK
            migrationBuilder.CreateTable(
                name: "movie_crew",
                columns: t => new
                {
                    Id = t.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = t.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = t.Column<Guid>(type: "uuid", nullable: false),
                    Department = t.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Job = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreditId = t.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_movie_crew", x => x.Id);
                    t.ForeignKey("FK_movie_crew_movies_MovieId", x => x.MovieId, "movies", "Id", onDelete: ReferentialAction.Cascade);
                    t.ForeignKey("FK_movie_crew_persons_PersonId", x => x.PersonId, "persons", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_MovieId_PersonId_CreditId",
                table: "movie_crew",
                columns: new[] { "MovieId", "PersonId", "CreditId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_PersonId",
                table: "movie_crew",
                column: "PersonId");

            // Recreate movie_genres with Guid GenreId FK
            migrationBuilder.CreateTable(
                name: "movie_genres",
                columns: t => new
                {
                    MovieId = t.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = t.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_movie_genres", x => new { x.MovieId, x.GenreId });
                    t.ForeignKey("FK_movie_genres_genres_GenreId", x => x.GenreId, "genres", "Id", onDelete: ReferentialAction.Restrict);
                    t.ForeignKey("FK_movie_genres_movies_MovieId", x => x.MovieId, "movies", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_genres_GenreId",
                table: "movie_genres",
                column: "GenreId");

            // Recreate movie_production_companies with Guid CompanyId FK
            migrationBuilder.CreateTable(
                name: "movie_production_companies",
                columns: t => new
                {
                    MovieId = t.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = t.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_movie_production_companies", x => new { x.MovieId, x.CompanyId });
                    t.ForeignKey("FK_movie_production_companies_movies_MovieId", x => x.MovieId, "movies", "Id", onDelete: ReferentialAction.Cascade);
                    t.ForeignKey("FK_movie_production_companies_production_companies_CompanyId", x => x.CompanyId, "production_companies", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_production_companies_CompanyId",
                table: "movie_production_companies",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "movie_cast");
            migrationBuilder.DropTable(name: "movie_crew");
            migrationBuilder.DropTable(name: "movie_genres");
            migrationBuilder.DropTable(name: "movie_production_companies");
            migrationBuilder.DropTable(name: "persons");
            migrationBuilder.DropTable(name: "genres");
            migrationBuilder.DropTable(name: "production_companies");

            migrationBuilder.RenameColumn(name: "IdTmdb", table: "movies", newName: "TmdbId");
            migrationBuilder.RenameIndex(name: "IX_movies_IdTmdb", table: "movies", newName: "IX_movies_TmdbId");

            migrationBuilder.CreateTable(
                name: "persons",
                columns: t => new
                {
                    Id = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfilePath = t.Column<string>(type: "text", nullable: true),
                    Biography = t.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Birthday = t.Column<DateOnly>(type: "date", nullable: true),
                    PlaceOfBirth = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Popularity = t.Column<double>(type: "double precision", nullable: true),
                    SyncedAt = t.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_persons", x => x.Id));

            migrationBuilder.CreateTable(
                name: "genres",
                columns: t => new
                {
                    Id = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_genres", x => x.Id));

            migrationBuilder.CreateTable(
                name: "production_companies",
                columns: t => new
                {
                    Id = t.Column<int>(type: "integer", nullable: false),
                    Name = t.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoPath = t.Column<string>(type: "text", nullable: true),
                    OriginCountry = t.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: t => t.PrimaryKey("PK_production_companies", x => x.Id));
        }
    }
}
