using CineLog.Domain.Entities;

namespace CineLog.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Review>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Review>> GetByMovieIdAsync(Guid movieId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateReactionsAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(Review review, CancellationToken cancellationToken = default);
    Task RemoveReactionAsync(ReviewReaction reaction, CancellationToken cancellationToken = default);
}
