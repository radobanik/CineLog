using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.UnitTests.TestHelpers;

/// <summary>
/// Lightweight DbContext backed by the InMemory provider for unit tests.
/// </summary>
public class TestAppDbContext : DbContext, IAppDbContext
{
    public TestAppDbContext(DbContextOptions<TestAppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewReaction> ReviewReactions => Set<ReviewReaction>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<MovieCast> MovieCast => Set<MovieCast>();
    public DbSet<MovieCrew> MovieCrew => Set<MovieCrew>();
    public DbSet<ProductionCompany> ProductionCompanies => Set<ProductionCompany>();
    public DbSet<MovieProductionCompany> MovieProductionCompanies => Set<MovieProductionCompany>();

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

        modelBuilder.Entity<MovieGenre>(b =>
        {
            b.HasKey(mg => new { mg.MovieId, mg.GenreId });
        });

        modelBuilder.Entity<MovieProductionCompany>(b =>
        {
            b.HasKey(mp => new { mp.MovieId, mp.CompanyId });
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
