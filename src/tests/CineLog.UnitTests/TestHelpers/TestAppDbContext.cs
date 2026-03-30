using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.UnitTests.TestHelpers;

/// <summary>
/// Lightweight DbContext backed by the InMemory provider for unit tests.
/// Omits PostgreSQL-specific config (jsonb, column types) while preserving entity structure.
/// </summary>
public class TestAppDbContext : DbContext, IAppDbContext
{
    public TestAppDbContext(DbContextOptions<TestAppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewReaction> ReviewReactions => Set<ReviewReaction>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.OwnsOne(u => u.Username, n => n.Property(x => x.Value));
        });

        modelBuilder.Entity<Movie>(b =>
        {
            b.HasKey(m => m.Id);
        });

        modelBuilder.Entity<Review>(b =>
        {
            b.HasKey(r => r.Id);
            b.OwnsOne(r => r.Rating, n => n.Property(x => x.Value));
            b.Ignore(r => r.DomainEvents);
        });

        modelBuilder.Entity<ReviewReaction>(b =>
        {
            b.HasKey(r => r.Id);
        });

        modelBuilder.Entity<UserFollow>(b =>
        {
            b.HasKey(f => new { f.FollowerId, f.FollowedId });
        });
    }

    public static TestAppDbContext Create(string? dbName = null)
    {
        var opts = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;
        return new TestAppDbContext(opts);
    }
}
