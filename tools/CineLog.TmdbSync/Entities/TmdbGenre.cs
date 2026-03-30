namespace CineLog.TmdbSync.Entities;

public class TmdbGenre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsMovieGenre { get; set; }
    public bool IsTvGenre { get; set; }
}
