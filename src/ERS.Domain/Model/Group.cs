namespace ERS.Domain.Model;

public sealed class Group
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = default!;   // "К-21"
    public int Course { get; init; }                        // 1..4
    public bool IsActive { get; init; } = true;
}