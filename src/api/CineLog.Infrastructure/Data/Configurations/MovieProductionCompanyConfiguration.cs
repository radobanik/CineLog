using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class MovieProductionCompanyConfiguration : IEntityTypeConfiguration<MovieProductionCompany>
{
    public void Configure(EntityTypeBuilder<MovieProductionCompany> builder)
    {
        builder.ToTable("movie_production_companies");
        builder.HasKey(mp => new { mp.MovieId, mp.CompanyId });

        builder.HasOne(mp => mp.Company)
            .WithMany()
            .HasForeignKey(mp => mp.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
