using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class ProductionCompanyConfiguration : IEntityTypeConfiguration<ProductionCompany>
{
    public void Configure(EntityTypeBuilder<ProductionCompany> builder)
    {
        builder.ToTable("production_companies");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.IdTmdb).IsRequired();
        builder.HasIndex(c => c.IdTmdb).IsUnique();
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.OriginCountry).HasMaxLength(10);
    }
}
