namespace ERS.Domain.Results;

public sealed record EffectiveSlot(
    int PairNumber,
    TimeOnly Start,
    TimeOnly End,
    SlotStatus Status,
    string? SubjectName,
    string? TeacherName,
    string? RoomNumber,
    string? Reason
);