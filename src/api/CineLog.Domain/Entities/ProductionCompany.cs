namespace CineLog.Domain.Entities;

public class ProductionCompany
{
    public Guid Id { get; set; }
    public int IdTmdb { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoPath { get; set; }
    public string? OriginCountry { get; set; }
}
