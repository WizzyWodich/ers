namespace ERS.Domain.Model;

public sealed class Room
{
    public Guid Id { get; init; }
    public string Number { get; init; } = default!;
    public string? Building { get; init; }
}