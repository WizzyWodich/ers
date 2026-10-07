using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace ERS.Infrastructure.Persistence;

public sealed class ErsDbContext : DbContext
{
    public ErsDbContext(DbContextOptions<ErsDbContext> options)
        : base(options) { }

    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BellSchedule> BellSchedules => Set<BellSchedule>();
    public DbSet<BaseScheduleEntry> BaseScheduleEntries => Set<BaseScheduleEntry>();
    public DbSet<ScheduleOverride> ScheduleOverrides => Set<ScheduleOverride>();
    public DbSet<CalendarDay> CalendarDays => Set<CalendarDay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ErsDbContext).Assembly);

        modelBuilder.Entity<BellSchedule>()
            .Ignore(b => b.PairSectors);
    }
}