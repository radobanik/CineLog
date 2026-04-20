namespace CineLog.Mobile.Core.Models.Search;

public sealed class PersonItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ProfilePath { get; init; }
}
