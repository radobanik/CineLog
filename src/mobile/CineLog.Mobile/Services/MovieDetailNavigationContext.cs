using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public class MovieDetailNavigationContext : IMovieDetailNavigationContext
{
    public Guid MovieId { get; set; }
}
