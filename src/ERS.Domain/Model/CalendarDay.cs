namespace ERS.Domain.Model;

public sealed class CalendarDay
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public bool IsShortened { get; init; }
    public DayOfWeek? ReplacedWeekday { get; init; }
    public Guid? BellScheduleId { get; init; }
    public bool IsHoliday { get; init; }
    public string? Note { get; init; }
}