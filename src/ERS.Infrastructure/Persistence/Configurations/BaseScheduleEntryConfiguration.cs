using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class BaseScheduleEntryConfiguration : IEntityTypeConfiguration<BaseScheduleEntry>
{
    public void Configure(EntityTypeBuilder<BaseScheduleEntry> builder)
    {
        builder.ToTable("base_schedule_entries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Weekday).IsRequired();
        builder.Property(e => e.PairNumber).IsRequired();
        builder.Property(e => e.ValidFrom).IsRequired();
        builder.Property(e => e.ValidTo);

        builder.HasIndex(e => new { e.GroupId, e.Weekday, e.PairNumber, e.ValidFrom })
            .IsUnique();

        builder.HasOne<Group>().WithMany().HasForeignKey(e => e.GroupId);
        builder.HasOne<Subject>().WithMany().HasForeignKey(e => e.SubjectId);
        builder.HasOne<Teacher>().WithMany().HasForeignKey(e => e.TeacherId);
        builder.HasOne<Room>().WithMany().HasForeignKey(e => e.RoomId);
        builder.HasOne<BellSchedule>().WithMany().HasForeignKey(e => e.BellScheduleId);
    }
}