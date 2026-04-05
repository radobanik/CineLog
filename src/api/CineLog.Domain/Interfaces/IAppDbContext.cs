using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Domain.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Movie> Movies { get; }
    DbSet<Review> Reviews { get; }
    DbSet<ReviewReaction> ReviewReactions { get; }
    DbSet<UserFollow> UserFollows { get; }
    DbSet<Watchlist> Watchlists { get; }
    DbSet<WatchlistItem> WatchlistItems { get; }
    DbSet<UserFavorite> UserFavorites { get; }
    DbSet<Genre> Genres { get; }
    DbSet<MovieGenre> MovieGenres { get; }
    DbSet<Person> Persons { get; }
    DbSet<MovieCast> MovieCast { get; }
    DbSet<MovieCrew> MovieCrew { get; }
    DbSet<ProductionCompany> ProductionCompanies { get; }
    DbSet<MovieProductionCompany> MovieProductionCompanies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
