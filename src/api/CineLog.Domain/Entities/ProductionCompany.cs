namespace CineLog.Domain.Entities;

public class ProductionCompany
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoPath { get; set; }
    public string? OriginCountry { get; set; }
}
