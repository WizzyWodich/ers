namespace ERS.Domain.Model;

public sealed class BellSchedule
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public IReadOnlyList<BellSector> Sectors { get; init; } = [];
    public bool IsActive { get; init; } = true;

    // Только Pair-сектора, отсортированные — на них опирается резолвер
    public IReadOnlyList<BellSector> PairSectors =>
        Sectors.Where(s => s.Type == BellSectorType.Pair)
               .OrderBy(s => s.Order)
               .ToList();
}