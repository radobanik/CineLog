using CineLog.Domain.Entities;
using CineLog.Infrastructure.Data;
using CineLog.TmdbSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Data;

public class TmdbSyncDbContext(DbContextOptions<TmdbSyncDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<MovieCast> MovieCast => Set<MovieCast>();
    public DbSet<MovieCrew> MovieCrew => Set<MovieCrew>();
    public DbSet<ProductionCompany> ProductionCompanies => Set<ProductionCompany>();
    public DbSet<MovieProductionCompany> MovieProductionCompanies => Set<MovieProductionCompany>();
    public DbSet<SyncCheckpoint> SyncCheckpoints => Set<SyncCheckpoint>();
    public DbSet<SyncFailure> SyncFailures => Set<SyncFailure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<SyncCheckpoint>(b =>
        {
            b.ToTable("sync_checkpoints");
            b.HasKey(c => c.SyncType);
            b.Property(c => c.SyncType).HasColumnName("sync_type").HasMaxLength(50);
            b.Property(c => c.LastPage).HasColumnName("last_page");
            b.Property(c => c.TotalPages).HasColumnName("total_pages");
            b.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<SyncFailure>(b =>
        {
            b.ToTable("sync_failures");
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).HasColumnName("id").ValueGeneratedOnAdd();
            b.Property(f => f.SyncType).HasColumnName("sync_type").HasMaxLength(50).IsRequired();
            b.Property(f => f.TmdbId).HasColumnName("tmdb_id");
            b.Property(f => f.ErrorMessage).HasColumnName("error_message").HasMaxLength(2000).IsRequired();
            b.Property(f => f.RetryCount).HasColumnName("retry_count");
            b.Property(f => f.FailedAt).HasColumnName("failed_at");
            b.Property(f => f.ResolvedAt).HasColumnName("resolved_at");
            b.HasIndex(f => new { f.SyncType, f.TmdbId, f.ResolvedAt });
        });
    }
}
