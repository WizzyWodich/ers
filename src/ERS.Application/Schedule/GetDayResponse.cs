namespace ERS.Application.Schedule.GetDay;

public sealed record GetDayResponse(
    DateOnly Date,
    string GroupDisplayName,
    string BellScheduleName,
    bool IsHoliday,
    bool IsShortened,
    string EffectiveWeekday,
    IReadOnlyList<GetDaySlot> Slots
);

public sealed record GetDaySlot(
    int PairNumber,
    string Start,
    string End,
    string Status,
    string? SubjectName,
    string? TeacherName,
    string? RoomNumber,
    string? Reason
);