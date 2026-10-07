namespace ERS.Domain.Model;

// Шаблон на неделю. PairNumber = НОМЕР СЛОТА, не порядковый номер пары.
public sealed class BaseScheduleEntry
{
    public Guid Id { get; init; }
    public Guid GroupId { get; init; }
    public DayOfWeek Weekday { get; init; }
    public int PairNumber { get; init; }
    public Guid SubjectId { get; init; }
    public Guid TeacherId { get; init; }
    public Guid RoomId { get; init; }
    public Guid BellScheduleId { get; init; }
    public DateOnly ValidFrom { get; init; }
    public DateOnly? ValidTo { get; init; }

    public bool IsActiveOn(DateOnly date) =>
        ValidFrom <= date && (ValidTo == null || ValidTo >= date);
}