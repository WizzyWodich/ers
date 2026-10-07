using System.Text.Json;
using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class BellScheduleConfiguration : IEntityTypeConfiguration<BellSchedule>
{
    public void Configure(EntityTypeBuilder<BellSchedule> builder)
    {
        builder.ToTable("bell_schedules");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
        builder.Property(b => b.IsActive).IsRequired();

        builder.Property(b => b.Sectors)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<BellSector>>(v, (JsonSerializerOptions?)null)!);
    }
}