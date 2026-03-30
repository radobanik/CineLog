namespace CineLog.Domain.Entities;

public class Genre
{
    public Guid Id { get; set; }
    public int IdTmdb { get; set; }
    public string Name { get; set; } = null!;
}
