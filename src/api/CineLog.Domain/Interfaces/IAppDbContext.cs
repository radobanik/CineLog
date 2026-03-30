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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
