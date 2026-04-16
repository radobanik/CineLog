using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public class MovieNavigationContext : IMovieNavigationContext
{
    public MovieCategory Category { get; set; }
}
