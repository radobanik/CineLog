using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IAppDbContext _context;

    public ReviewRepository(IAppDbContext context) => _context = context;

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Reviews
            .Include(r => r.Reactions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Review>> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var results = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Review>> GetByMovieIdAsync(
        Guid movieId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var results = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.MovieId == movieId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        var entry = ((DbContext)_context).Entry(review);
        if (entry.State == EntityState.Unchanged)
            entry.State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateReactionsAsync(Review review, CancellationToken cancellationToken = default)
    {
        var dbContext = (DbContext)_context;
        var currentIds = review.Reactions.Select(r => r.Id).ToList();
        
        await _context.ReviewReactions
            .Where(r => r.ReviewId == review.Id && !currentIds.Contains(r.Id))
            .ExecuteDeleteAsync(cancellationToken);

        dbContext.ChangeTracker.Clear();

        var existingIds = await _context.ReviewReactions
            .Where(r => r.ReviewId == review.Id)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        foreach (var reaction in review.Reactions.Where(r => !existingIds.Contains(r.Id)))
            _context.ReviewReactions.Add(reaction);

        await _context.SaveChangesAsync(cancellationToken);

        var likesCount = await _context.ReviewReactions
            .CountAsync(rr => rr.ReviewId == review.Id && rr.Type == Domain.Enums.ReactionType.Like, cancellationToken);

        await _context.Reviews
            .Where(r => r.Id == review.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.LikesCount, likesCount), cancellationToken);
    }

    public async Task DeleteAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveReactionAsync(ReviewReaction reaction, CancellationToken cancellationToken = default)
    {
        _context.ReviewReactions.Remove(reaction);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
