namespace CineLog.Domain.Entities;

public class MovieCrew
{
    public long Id { get; set; }
    public Guid MovieId { get; set; }
    public int PersonId { get; set; }
    public string? Department { get; set; }
    public string? Job { get; set; }
    public string? CreditId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Person Person { get; set; } = null!;
}
