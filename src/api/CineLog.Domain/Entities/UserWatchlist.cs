namespace CineLog.Domain.Entities;

public class Watchlist
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<WatchlistItem> _items = [];
    public IReadOnlyCollection<WatchlistItem> Items => _items.AsReadOnly();

    private Watchlist() { }

    public static Watchlist Create(Guid userId, string name)
    {
        return new Watchlist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}

public class WatchlistItem
{
    public Guid WatchlistId { get; private set; }
    public Guid MovieId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }

    private WatchlistItem() { }

    public static WatchlistItem Create(Guid watchlistId, Guid movieId)
    {
        return new WatchlistItem
        {
            WatchlistId = watchlistId,
            MovieId = movieId,
            AddedAt = DateTimeOffset.UtcNow
        };
    }
}
