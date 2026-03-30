using CineLog.Domain.Entities;
using CineLog.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Data;

public class TmdbSyncDbContext : DbContext
{
    public TmdbSyncDbContext(DbContextOptions<TmdbSyncDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MovieConfiguration());
    }
}
