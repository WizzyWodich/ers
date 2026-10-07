using ERS.Domain.Model;
using ERS.Domain.Results;

namespace ERS.Domain.Tests.Builders;

// Билдер для ResolveContext. Позволяет собирать контекст одной цепочкой.
public sealed class ScheduleContextBuilder
{
    private Guid _groupId = Guid.NewGuid();
    private string _groupName = "К-21";
    private DateOnly _date = new(2025, 10, 14); // вторник
    private BellSchedule _bellSchedule = default!;
    private CalendarDay? _calendarDay;

    private readonly Dictionary<Guid, Subject> _subjects = new();
    private readonly Dictionary<Guid, Teacher> _teachers = new();
    private readonly Dictionary<Guid, Room> _rooms = new();

    private readonly List<BaseScheduleEntry> _baseEntries = new();
    private readonly List<ScheduleOverride> _overridesOut = new();
    private readonly List<ScheduleOverride> _overridesIn = new();

    // Справочники — заранее созданные, чтобы ссылаться в тестах по имени
    public static readonly Guid MathId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ProgId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PhysId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid PetrovId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid SidorovId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid IvanovId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public static readonly Guid Room301 = Guid.Parse("d1111111-1111-1111-1111-111111111111");
    public static readonly Guid Room302 = Guid.Parse("d2222222-2222-2222-2222-222222222222");
    public static readonly Guid SportZal = Guid.Parse("d3333333-3333-3333-3333-333333333333");

    public ScheduleContextBuilder()
    {
        // Справочники по умолчанию
        _subjects[MathId]   = new Subject { Id = MathId,   Name = "Математика",       ShortName = "Матан" };
        _subjects[ProgId]   = new Subject { Id = ProgId,   Name = "Программирование", ShortName = "Прог" };
        _subjects[PhysId]   = new Subject { Id = PhysId,   Name = "Физкультура",      ShortName = "Физра" };

        _teachers[PetrovId]  = new Teacher { Id = PetrovId,  FullName = "Петров Иван Сергеевич",   ShortName = "Петров И.С." };
        _teachers[SidorovId] = new Teacher { Id = SidorovId, FullName = "Сидоров Алексей Викторович", ShortName = "Сидоров А.В." };
        _teachers[IvanovId]  = new Teacher { Id = IvanovId,  FullName = "Иванов Пётр Петрович",    ShortName = "Иванов П.П." };

        _rooms[Room301]  = new Room { Id = Room301,  Number = "301" };
        _rooms[Room302]  = new Room { Id = Room302,  Number = "302" };
        _rooms[SportZal] = new Room { Id = SportZal, Number = "Спортзал" };

        // По умолчанию — обычное звонковое расписание, 4 пары
        _bellSchedule = StandardBellSchedule(Guid.NewGuid());
    }

    // === Настройки ===

    public ScheduleContextBuilder OnDate(int year, int month, int day)
    {
        _date = new DateOnly(year, month, day);
        return this;
    }

    public ScheduleContextBuilder WithCalendarDay(CalendarDay day)
    {
        _calendarDay = day;
        return this;
    }

    public ScheduleContextBuilder WithBellSchedule(BellSchedule schedule)
    {
        _bellSchedule = schedule;
        return this;
    }

    // === Базовая пара ===

    public ScheduleContextBuilder WithBasePair(
        DayOfWeek weekday,
        int pairNumber,
        Guid subjectId,
        Guid teacherId,
        Guid roomId)
    {
        _baseEntries.Add(new BaseScheduleEntry
        {
            Id = Guid.NewGuid(),
            GroupId = _groupId,
            Weekday = weekday,
            PairNumber = pairNumber,
            SubjectId = subjectId,
            TeacherId = teacherId,
            RoomId = roomId,
            BellScheduleId = _bellSchedule.Id,
            ValidFrom = new DateOnly(2025, 9, 1),
            ValidTo = null
        });
        return this;
    }

    // === Замены ===

    public ScheduleContextBuilder WithOverrideOut(ScheduleOverride ov)
    {
        _overridesOut.Add(ov);
        return this;
    }

    public ScheduleContextBuilder WithOverrideIn(ScheduleOverride ov)
    {
        _overridesIn.Add(ov);
        return this;
    }

    // Удобные фабрики для замен
    public static ScheduleOverride Cancelled(DateOnly date, int pairNumber, string? reason = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetDate = date,
            PairNumber = pairNumber,
            Type = OverrideType.Cancelled,
            Reason = reason,
            Status = OverrideStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

    public static ScheduleOverride SubstitutedTeacher(DateOnly date, int pairNumber, Guid newTeacherId, string? reason = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetDate = date,
            PairNumber = pairNumber,
            Type = OverrideType.Substituted,
            NewTeacherId = newTeacherId,
            Reason = reason,
            Status = OverrideStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

    public static ScheduleOverride Added(DateOnly date, int pairNumber, Guid subjectId, Guid teacherId, Guid roomId) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetDate = date,
            PairNumber = pairNumber,
            Type = OverrideType.Added,
            NewSubjectId = subjectId,
            NewTeacherId = teacherId,
            NewRoomId = roomId,
            Status = OverrideStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

    public static ScheduleOverride MovedOut(DateOnly fromDate, int fromPair, DateOnly toDate, int toPair) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetDate = fromDate,
            PairNumber = fromPair,
            Type = OverrideType.Moved,
            NewDate = toDate,
            NewPairNumber = toPair,
            Status = OverrideStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

    public static ScheduleOverride MovedIn(DateOnly fromDate, int fromPair, DateOnly toDate, int toPair) =>
        MovedOut(fromDate, fromPair, toDate, toPair);

    // === Сборка ===

    public ResolveContext Build() =>
        new(
            new Group { Id = _groupId, DisplayName = _groupName, Course = 2 },
            _date,
            _bellSchedule,
            _calendarDay,
            _subjects,
            _teachers,
            _rooms,
            _overridesOut,
            _overridesIn,
            _baseEntries);

    // === Стандартное звонковое расписание (4 пары) ===

    public static BellSchedule StandardBellSchedule(Guid id) =>
        new()
        {
            Id = id,
            Name = "Обычное",
            Sectors = new List<BellSector>
            {
                new(1, BellSectorType.Pair,     new TimeOnly(8, 0),  new TimeOnly(9, 20)),
                new(2, BellSectorType.Break,    new TimeOnly(9, 20), new TimeOnly(9, 30)),
                new(3, BellSectorType.Pair,     new TimeOnly(9, 30), new TimeOnly(10, 50)),
                new(4, BellSectorType.BigBreak, new TimeOnly(10, 50), new TimeOnly(11, 40)),
                new(5, BellSectorType.Pair,     new TimeOnly(11, 40), new TimeOnly(13, 0)),
                new(6, BellSectorType.Break,    new TimeOnly(13, 0), new TimeOnly(13, 10)),
                new(7, BellSectorType.Pair,     new TimeOnly(13, 10), new TimeOnly(14, 30))
            }
        };

    // Сокращённое — пары по часу, перемены по 5 минут, без большой
    public static BellSchedule ShortenedBellSchedule(Guid id) =>
        new()
        {
            Id = id,
            Name = "Сокращённое",
            Sectors = new List<BellSector>
            {
                new(1, BellSectorType.Pair,  new TimeOnly(8, 0),   new TimeOnly(9, 0)),
                new(2, BellSectorType.Break, new TimeOnly(9, 0),   new TimeOnly(9, 5)),
                new(3, BellSectorType.Pair,  new TimeOnly(9, 5),   new TimeOnly(10, 5)),
                new(4, BellSectorType.Break, new TimeOnly(10, 5),  new TimeOnly(10, 10)),
                new(5, BellSectorType.Pair,  new TimeOnly(10, 10), new TimeOnly(11, 10)),
                new(6, BellSectorType.Break, new TimeOnly(11, 10), new TimeOnly(11, 15)),
                new(7, BellSectorType.Pair,  new TimeOnly(11, 15), new TimeOnly(12, 15))
            }
        };
}