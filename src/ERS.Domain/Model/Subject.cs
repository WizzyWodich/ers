namespace ERS.Domain.Model;

public sealed class Subject
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string ShortName { get; init; } = default!;
}
