namespace ERS.Domain.Model;

public sealed class ScheduleOverride
{
    public Guid Id { get; init; }
    public DateOnly TargetDate { get; init; }
    public Guid GroupId { get; init; }
    public int PairNumber { get; init; }        // НОМЕР СЛОТА, не порядковый
    public OverrideType Type { get; init; }

    // null = не меняем соответствующее поле
    public Guid? NewSubjectId { get; init; }
    public Guid? NewTeacherId { get; init; }
    public Guid? NewRoomId { get; init; }

    // для Moved
    public DateOnly? NewDate { get; init; }
    public int? NewPairNumber { get; init; }

    public string? Reason { get; init; }
    public OverrideStatus Status { get; init; }
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
}