using CineLog.Domain.Entities;
using CineLog.Infrastructure.Data;
using CineLog.TmdbSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Data;

public class TmdbSyncDbContext(DbContextOptions<TmdbSyncDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
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

        // Reuse all entity configurations from Infrastructure
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Sync-only operational tables (not in AppDbContext)
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
