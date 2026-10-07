using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.FullName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.ShortName).IsRequired().HasMaxLength(100);
    }
}