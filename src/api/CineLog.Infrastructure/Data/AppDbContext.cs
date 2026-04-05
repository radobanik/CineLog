using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
{
    private readonly SecondLevelCacheInterceptor _cacheInterceptor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        SecondLevelCacheInterceptor cacheInterceptor) : base(options)
    {
        _cacheInterceptor = cacheInterceptor;
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewReaction> ReviewReactions => Set<ReviewReaction>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    public DbSet<Watchlist> Watchlists => Set<Watchlist>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<MovieCast> MovieCast => Set<MovieCast>();
    public DbSet<MovieCrew> MovieCrew => Set<MovieCrew>();
    public DbSet<ProductionCompany> ProductionCompanies => Set<ProductionCompany>();
    public DbSet<MovieProductionCompany> MovieProductionCompanies => Set<MovieProductionCompany>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_cacheInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
