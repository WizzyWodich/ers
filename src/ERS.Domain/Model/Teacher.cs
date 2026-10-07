namespace ERS.Domain.Model;

public sealed class Teacher
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
    public string ShortName { get; init; } = default!;
}