using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.IdTmdb).IsRequired();
        builder.HasIndex(p => p.IdTmdb).IsUnique();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Biography).HasMaxLength(5000);
        builder.Property(p => p.PlaceOfBirth).HasMaxLength(200);
    }
}
