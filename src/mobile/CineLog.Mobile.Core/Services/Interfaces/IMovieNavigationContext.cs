using CineLog.Mobile.Core.Models;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IMovieNavigationContext
{
    MovieCategory Category { get; set; }
}
