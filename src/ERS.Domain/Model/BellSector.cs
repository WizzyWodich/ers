namespace ERS.Domain.Model;

public sealed record BellSector(
    int Order,
    BellSectorType Type,
    TimeOnly Start,
    TimeOnly End
);