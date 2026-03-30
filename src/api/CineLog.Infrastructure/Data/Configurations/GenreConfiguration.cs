using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.IdTmdb).IsRequired();
        builder.HasIndex(g => g.IdTmdb).IsUnique();
        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
    }
}
