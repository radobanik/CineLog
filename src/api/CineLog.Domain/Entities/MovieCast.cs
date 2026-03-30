namespace CineLog.Domain.Entities;

public class MovieCast
{
    public long Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public string? Character { get; set; }
    public int Order { get; set; }
    public string? CreditId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Person Person { get; set; } = null!;
}
