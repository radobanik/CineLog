namespace CineLog.Mobile.Core.Models.Movies;

public sealed class CastMemberItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Character { get; init; }
    public string? ProfilePath { get; init; }
    public string Initial => Name.Length > 0 ? Name[0].ToString().ToUpper() : "?";
    public string FirstName => Name.Split(' ')[0];
}
