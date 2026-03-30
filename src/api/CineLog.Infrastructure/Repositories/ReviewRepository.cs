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
            .AsNoTracking()
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
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
