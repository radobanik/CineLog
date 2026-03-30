namespace CineLog.Domain.Entities;

public class MovieProductionCompany
{
    public Guid MovieId { get; set; }
    public Guid CompanyId { get; set; }

    public Movie Movie { get; set; } = null!;
    public ProductionCompany Company { get; set; } = null!;
}
