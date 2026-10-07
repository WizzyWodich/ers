using ERS.Domain.Model;
using ERS.Domain.Results;
using ERS.Domain.Services;
using ERS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERS.Application.Schedule.GetDay;

public sealed class GetDayHandler
{
    private readonly ErsDbContext _db;
    private readonly ScheduleResolver _resolver;

    public GetDayHandler(ErsDbContext db, ScheduleResolver resolver)
    {
        _db = db;
        _resolver = resolver;
    }

    public async Task<GetDayResponse?> HandleAsync(
        Guid groupId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var group = await _db.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId, ct);

        if (group is null)
            return null;

        var calendarDay = await _db.CalendarDays
            .FirstOrDefaultAsync(c => c.Date == date, ct);

        var effectiveWeekday = calendarDay?.ReplacedWeekday ?? date.DayOfWeek;

        BellSchedule bellSchedule;

        if (calendarDay?.BellScheduleId is Guid bsId)
        {
            bellSchedule = await _db.BellSchedules
                .FirstAsync(b => b.Id == bsId, ct);
        }
        else if (calendarDay?.IsShortened == true)
        {
            bellSchedule = await _db.BellSchedules
                .FirstAsync(b => b.Name == "Сокращённое", ct);
        }
        else
        {
            bellSchedule = await _db.BellSchedules
                .FirstAsync(b => b.Name == "Обычное", ct);
        }

        var baseEntries = await _db.BaseScheduleEntries
            .Where(e => e.GroupId == groupId
                     && e.Weekday == effectiveWeekday
                     && e.ValidFrom <= date
                     && (e.ValidTo == null || e.ValidTo >= date))
            .ToListAsync(ct);

        var overridesOut = await _db.ScheduleOverrides
            .Where(o => o.GroupId == groupId
                     && o.TargetDate == date
                     && o.Status == OverrideStatus.Published)
            .ToListAsync(ct);

        var overridesIn = await _db.ScheduleOverrides
            .Where(o => o.GroupId == groupId
                     && o.NewDate == date
                     && o.Type == OverrideType.Moved
                     && o.Status == OverrideStatus.Published)
            .ToListAsync(ct);

        var subjects = await _db.Subjects
            .ToDictionaryAsync(s => s.Id, ct);
        var teachers = await _db.Teachers
            .ToDictionaryAsync(t => t.Id, ct);
        var rooms = await _db.Rooms
            .ToDictionaryAsync(r => r.Id, ct);

        var ctx = new ResolveContext(
            group,
            date,
            bellSchedule,
            calendarDay,
            subjects,
            teachers,
            rooms,
            overridesOut,
            overridesIn,
            baseEntries);

        var effective = _resolver.Resolve(ctx);

        return MapToResponse(effective);
    }

    private static GetDayResponse MapToResponse(EffectiveSchedule s) =>
        new(
            s.Date,
            s.GroupDisplayName,
            s.BellScheduleName,
            s.IsHoliday,
            s.IsShortened,
            s.EffectiveWeekday.ToString(),
            s.Slots.Select(slot => new GetDaySlot(
                slot.PairNumber,
                slot.Start.ToString("HH:mm"),
                slot.End.ToString("HH:mm"),
                slot.Status.ToString(),
                slot.SubjectName,
                slot.TeacherName,
                slot.RoomNumber,
                slot.Reason
            )).ToList()
        );
}