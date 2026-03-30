using CineLog.TmdbSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Data;

public class TmdbSchemaDbContext(DbContextOptions<TmdbSchemaDbContext> options) : DbContext(options)
{
    public DbSet<TmdbGenre> Genres => Set<TmdbGenre>();
    public DbSet<TmdbPerson> Persons => Set<TmdbPerson>();
    public DbSet<TmdbProductionCompany> ProductionCompanies => Set<TmdbProductionCompany>();
    public DbSet<TmdbMovieDetail> MovieDetails => Set<TmdbMovieDetail>();
    public DbSet<TmdbTvSeries> TvSeries => Set<TmdbTvSeries>();
    public DbSet<TmdbMovieGenre> MovieGenres => Set<TmdbMovieGenre>();
    public DbSet<TmdbTvGenre> TvGenres => Set<TmdbTvGenre>();
    public DbSet<TmdbMovieCast> MovieCast => Set<TmdbMovieCast>();
    public DbSet<TmdbMovieCrew> MovieCrew => Set<TmdbMovieCrew>();
    public DbSet<TmdbTvCast> TvCast => Set<TmdbTvCast>();
    public DbSet<TmdbTvCrew> TvCrew => Set<TmdbTvCrew>();
    public DbSet<TmdbMovieProductionCompany> MovieProductionCompanies => Set<TmdbMovieProductionCompany>();
    public DbSet<TmdbTvProductionCompany> TvProductionCompanies => Set<TmdbTvProductionCompany>();
    public DbSet<SyncCheckpoint> SyncCheckpoints => Set<SyncCheckpoint>();
    public DbSet<SyncFailure> SyncFailures => Set<SyncFailure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("tmdb");

        modelBuilder.Entity<TmdbGenre>(b =>
        {
            b.ToTable("genres");
            b.HasKey(g => g.Id);
            b.Property(g => g.Id).ValueGeneratedNever();
            b.Property(g => g.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<TmdbPerson>(b =>
        {
            b.ToTable("persons");
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).ValueGeneratedNever();
            b.Property(p => p.Name).HasMaxLength(200).IsRequired();
            b.Property(p => p.Biography).HasMaxLength(5000);
            b.Property(p => p.PlaceOfBirth).HasMaxLength(200);
        });

        modelBuilder.Entity<TmdbProductionCompany>(b =>
        {
            b.ToTable("production_companies");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedNever();
            b.Property(c => c.Name).HasMaxLength(200).IsRequired();
            b.Property(c => c.OriginCountry).HasMaxLength(10);
        });

        modelBuilder.Entity<TmdbMovieDetail>(b =>
        {
            b.ToTable("movie_details");
            b.HasKey(m => m.TmdbId);
            b.Property(m => m.TmdbId).ValueGeneratedNever();
            b.Property(m => m.ImdbId).HasMaxLength(20);
            b.Property(m => m.OriginalTitle).HasMaxLength(500);
            b.Property(m => m.OriginalLanguage).HasMaxLength(10);
            b.Property(m => m.Tagline).HasMaxLength(500);
            b.Property(m => m.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TmdbTvSeries>(b =>
        {
            b.ToTable("tv_series");
            b.HasKey(t => t.TmdbId);
            b.Property(t => t.TmdbId).ValueGeneratedNever();
            b.Property(t => t.Title).HasMaxLength(500).IsRequired();
            b.Property(t => t.OriginalLanguage).HasMaxLength(10);
            b.Property(t => t.Status).HasMaxLength(50);
            b.Property(t => t.Tagline).HasMaxLength(500);
            b.Property(t => t.AverageRating).HasPrecision(4, 2);
        });

        modelBuilder.Entity<TmdbMovieGenre>(b =>
        {
            b.ToTable("movie_genres");
            b.HasKey(mg => new { mg.TmdbMovieId, mg.GenreId });
        });

        modelBuilder.Entity<TmdbTvGenre>(b =>
        {
            b.ToTable("tv_genres");
            b.HasKey(tg => new { tg.TmdbTvId, tg.GenreId });
        });

        modelBuilder.Entity<TmdbMovieCast>(b =>
        {
            b.ToTable("movie_cast");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedOnAdd();
            b.Property(c => c.Character).HasMaxLength(500);
            b.Property(c => c.CreditId).HasMaxLength(50);
            b.HasIndex(c => new { c.TmdbMovieId, c.PersonId, c.CreditId }).IsUnique();
        });

        modelBuilder.Entity<TmdbMovieCrew>(b =>
        {
            b.ToTable("movie_crew");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedOnAdd();
            b.Property(c => c.Department).HasMaxLength(100);
            b.Property(c => c.Job).HasMaxLength(200);
            b.Property(c => c.CreditId).HasMaxLength(50);
            b.HasIndex(c => new { c.TmdbMovieId, c.PersonId, c.CreditId }).IsUnique();
        });

        modelBuilder.Entity<TmdbTvCast>(b =>
        {
            b.ToTable("tv_cast");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedOnAdd();
            b.Property(c => c.Character).HasMaxLength(500);
            b.Property(c => c.CreditId).HasMaxLength(50);
            b.HasIndex(c => new { c.TmdbTvId, c.PersonId, c.CreditId }).IsUnique();
        });

        modelBuilder.Entity<TmdbTvCrew>(b =>
        {
            b.ToTable("tv_crew");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedOnAdd();
            b.Property(c => c.Department).HasMaxLength(100);
            b.Property(c => c.Job).HasMaxLength(200);
            b.Property(c => c.CreditId).HasMaxLength(50);
            b.HasIndex(c => new { c.TmdbTvId, c.PersonId, c.CreditId }).IsUnique();
        });

        modelBuilder.Entity<TmdbMovieProductionCompany>(b =>
        {
            b.ToTable("movie_production_companies");
            b.HasKey(mp => new { mp.TmdbMovieId, mp.CompanyId });
        });

        modelBuilder.Entity<TmdbTvProductionCompany>(b =>
        {
            b.ToTable("tv_production_companies");
            b.HasKey(tp => new { tp.TmdbTvId, tp.CompanyId });
        });

        modelBuilder.Entity<SyncCheckpoint>(b =>
        {
            b.ToTable("sync_checkpoints");
            b.HasKey(c => c.SyncType);
            b.Property(c => c.SyncType).HasMaxLength(50);
        });

        modelBuilder.Entity<SyncFailure>(b =>
        {
            b.ToTable("sync_failures");
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).ValueGeneratedOnAdd();
            b.Property(f => f.SyncType).HasMaxLength(50).IsRequired();
            b.Property(f => f.ErrorMessage).HasMaxLength(2000).IsRequired();
            b.HasIndex(f => new { f.SyncType, f.TmdbId, f.ResolvedAt });
        });
    }
}
