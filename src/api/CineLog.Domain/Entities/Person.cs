namespace CineLog.Domain.Entities;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ProfilePath { get; set; }
    public string? Biography { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? PlaceOfBirth { get; set; }
    public double? Popularity { get; set; }
    public DateTimeOffset SyncedAt { get; set; }
}
