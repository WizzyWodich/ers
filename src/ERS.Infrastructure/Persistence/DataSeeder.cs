using ERS.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace ERS.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(ErsDbContext db)
    {
        // Если уже есть группы — считаем, что seed уже был, выходим
        if (await db.Groups.AnyAsync())
            return;

        // === Предметы ===
        var prog = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "Программирование",
            ShortName = "Прог"
        };
        var math = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "Математика",
            ShortName = "Матан"
        };
        var phys = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "Физкультура",
            ShortName = "Физра"
        };
        var hist = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "История",
            ShortName = "История"
        };

        // === Преподаватели ===
        var petrov = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Петров Иван Сергеевич",
            ShortName = "Петров И.С."
        };
        var sidorov = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Сидоров Алексей Викторович",
            ShortName = "Сидоров А.В."
        };
        var ivanov = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Иванов Пётр Петрович",
            ShortName = "Иванов П.П."
        };
        var kuznetsova = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Кузнецова Мария Ивановна",
            ShortName = "Кузнецова М.И."
        };

        // === Аудитории ===
        var room301 = new Room { Id = Guid.NewGuid(), Number = "301" };
        var room302 = new Room { Id = Guid.NewGuid(), Number = "302" };
        var sportzal = new Room { Id = Guid.NewGuid(), Number = "Спортзал" };
        var room305 = new Room { Id = Guid.NewGuid(), Number = "305" };

        // === Звонковое расписание "Обычное" (4 пары) ===
        var standardBell = new BellSchedule
        {
            Id = Guid.NewGuid(),
            Name = "Обычное",
            IsActive = true,
            Sectors = new List<BellSector>
            {
                new(1, BellSectorType.Pair,     new TimeOnly(8, 0),   new TimeOnly(9, 20)),
                new(2, BellSectorType.Break,    new TimeOnly(9, 20),  new TimeOnly(9, 30)),
                new(3, BellSectorType.Pair,     new TimeOnly(9, 30),  new TimeOnly(10, 50)),
                new(4, BellSectorType.BigBreak, new TimeOnly(10, 50), new TimeOnly(11, 40)),
                new(5, BellSectorType.Pair,     new TimeOnly(11, 40), new TimeOnly(13, 0)),
                new(6, BellSectorType.Break,    new TimeOnly(13, 0),  new TimeOnly(13, 10)),
                new(7, BellSectorType.Pair,     new TimeOnly(13, 10), new TimeOnly(14, 30))
            }
        };

        // === Звонковое расписание "Сокращённое" ===
        var shortenedBell = new BellSchedule
        {
            Id = Guid.NewGuid(),
            Name = "Сокращённое",
            IsActive = true,
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

        // === Группа К-21 ===
        var group = new Group
        {
            Id = Guid.NewGuid(),
            DisplayName = "К-21",
            Course = 2,
            IsActive = true
        };

        // === Базовое расписание для К-21 на вторник (4 пары) ===
        var validFrom = new DateOnly(2025, 9, 1);

        var entries = new List<BaseScheduleEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                Weekday = DayOfWeek.Tuesday,
                PairNumber = 1,
                SubjectId = prog.Id,
                TeacherId = petrov.Id,
                RoomId = room301.Id,
                BellScheduleId = standardBell.Id,
                ValidFrom = validFrom
            },
            new()
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                Weekday = DayOfWeek.Tuesday,
                PairNumber = 2,
                SubjectId = math.Id,
                TeacherId = sidorov.Id,
                RoomId = room302.Id,
                BellScheduleId = standardBell.Id,
                ValidFrom = validFrom
            },
            new()
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                Weekday = DayOfWeek.Tuesday,
                PairNumber = 3,
                SubjectId = phys.Id,
                TeacherId = ivanov.Id,
                RoomId = sportzal.Id,
                BellScheduleId = standardBell.Id,
                ValidFrom = validFrom
            },
            new()
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                Weekday = DayOfWeek.Tuesday,
                PairNumber = 4,
                SubjectId = hist.Id,
                TeacherId = kuznetsova.Id,
                RoomId = room305.Id,
                BellScheduleId = standardBell.Id,
                ValidFrom = validFrom
            }
        };

        // === Сохраняем всё ===
        db.Subjects.AddRange(prog, math, phys, hist);
        db.Teachers.AddRange(petrov, sidorov, ivanov, kuznetsova);
        db.Rooms.AddRange(room301, room302, sportzal, room305);
        db.BellSchedules.AddRange(standardBell, shortenedBell);
        db.Groups.Add(group);
        db.BaseScheduleEntries.AddRange(entries);

        await db.SaveChangesAsync();
    }
}