namespace CineLog.TmdbSync.Entities;

public class TmdbMovieGenre
{
    public int TmdbMovieId { get; set; }
    public int GenreId { get; set; }
}

public class TmdbTvGenre
{
    public int TmdbTvId { get; set; }
    public int GenreId { get; set; }
}

public class TmdbMovieCast
{
    public long Id { get; set; }
    public int TmdbMovieId { get; set; }
    public int PersonId { get; set; }
    public string? Character { get; set; }
    public int? Order { get; set; }
    public string? CreditId { get; set; }
}

public class TmdbMovieCrew
{
    public long Id { get; set; }
    public int TmdbMovieId { get; set; }
    public int PersonId { get; set; }
    public string? Department { get; set; }
    public string? Job { get; set; }
    public string? CreditId { get; set; }
}

public class TmdbTvCast
{
    public long Id { get; set; }
    public int TmdbTvId { get; set; }
    public int PersonId { get; set; }
    public string? Character { get; set; }
    public int? Order { get; set; }
    public string? CreditId { get; set; }
}

public class TmdbTvCrew
{
    public long Id { get; set; }
    public int TmdbTvId { get; set; }
    public int PersonId { get; set; }
    public string? Department { get; set; }
    public string? Job { get; set; }
    public string? CreditId { get; set; }
}

public class TmdbMovieProductionCompany
{
    public int TmdbMovieId { get; set; }
    public int CompanyId { get; set; }
}

public class TmdbTvProductionCompany
{
    public int TmdbTvId { get; set; }
    public int CompanyId { get; set; }
}
