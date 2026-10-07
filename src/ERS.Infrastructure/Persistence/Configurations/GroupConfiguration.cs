using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.DisplayName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.Course)
            .IsRequired();

        builder.Property(g => g.IsActive)
            .IsRequired();

        builder.HasIndex(g => g.DisplayName)
            .IsUnique();
    }
}