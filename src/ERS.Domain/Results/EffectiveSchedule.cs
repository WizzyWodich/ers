namespace ERS.Domain.Results;

public sealed record EffectiveSchedule(
    DateOnly Date,
    string GroupDisplayName,
    string BellScheduleName,
    bool IsHoliday,
    bool IsShortened,
    DayOfWeek EffectiveWeekday,
    IReadOnlyList<EffectiveSlot> Slots
);