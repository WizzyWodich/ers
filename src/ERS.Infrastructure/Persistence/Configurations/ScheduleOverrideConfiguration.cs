using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class ScheduleOverrideConfiguration : IEntityTypeConfiguration<ScheduleOverride>
{
    public void Configure(EntityTypeBuilder<ScheduleOverride> builder)
    {
        builder.ToTable("schedule_overrides");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TargetDate).IsRequired();
        builder.Property(o => o.PairNumber).IsRequired();
        builder.Property(o => o.Type).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.Reason).HasMaxLength(500);
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.HasIndex(o => new { o.TargetDate, o.GroupId, o.PairNumber })
            .IsUnique()
            .HasFilter("\"Status\" = 1"); 

        builder.HasOne<Group>().WithMany().HasForeignKey(o => o.GroupId);
        builder.HasOne<Subject>().WithMany().HasForeignKey(o => o.NewSubjectId);
        builder.HasOne<Teacher>().WithMany().HasForeignKey(o => o.NewTeacherId);
        builder.HasOne<Room>().WithMany().HasForeignKey(o => o.NewRoomId);
    }
}