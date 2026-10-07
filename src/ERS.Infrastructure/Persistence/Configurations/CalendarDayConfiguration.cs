using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERS.Infrastructure.Persistence.Configurations;

public sealed class CalendarDayConfiguration : IEntityTypeConfiguration<CalendarDay>
{
    public void Configure(EntityTypeBuilder<CalendarDay> builder)
    {
        builder.ToTable("calendar_days");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Date).IsRequired();
        builder.HasIndex(c => c.Date).IsUnique();

        builder.Property(c => c.Note).HasMaxLength(500);

        builder.HasOne<BellSchedule>().WithMany().HasForeignKey(c => c.BellScheduleId);
    }
}