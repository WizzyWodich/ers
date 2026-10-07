using ERS.Domain.Model;

namespace ERS.Domain.Results;

public sealed record ResolveContext(
    Group Group,
    DateOnly Date,
    BellSchedule BellSchedule,
    CalendarDay? CalendarDay,
    IReadOnlyDictionary<Guid, Subject> Subjects,
    IReadOnlyDictionary<Guid, Teacher> Teachers,
    IReadOnlyDictionary<Guid, Room> Rooms,

    IReadOnlyList<ScheduleOverride> OverridesOut,

    IReadOnlyList<ScheduleOverride> OverridesIn,

    IReadOnlyList<BaseScheduleEntry> BaseEntries
);