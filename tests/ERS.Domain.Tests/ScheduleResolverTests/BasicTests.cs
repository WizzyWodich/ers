namespace ERS.Domain.Tests.ScheduleResolverTests;

public class BasicTests
{
    private readonly ScheduleResolver _resolver = new();

    // === 1. Обычный день без замен — все слоты Normal ===
    [Fact]
    public void Normal_day_no_overrides_returns_all_slots()
    {
        var date = new DateOnly(2025, 10, 14); // вторник
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithBasePair(DayOfWeek.Tuesday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .WithBasePair(DayOfWeek.Tuesday, 3, ScheduleContextBuilder.PhysId, ScheduleContextBuilder.IvanovId, ScheduleContextBuilder.SportZal)
            .WithBasePair(DayOfWeek.Tuesday, 4, ScheduleContextBuilder.MathId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.Equal(4, result.Slots.Count);
        Assert.All(result.Slots, s => Assert.Equal(SlotStatus.Normal, s.Status));
        Assert.False(result.IsHoliday);
        Assert.False(result.IsShortened);

        // времена не сдвинуты
        Assert.Equal(new TimeOnly(8, 0),   result.Slots[0].Start);
        Assert.Equal(new TimeOnly(9, 30),  result.Slots[1].Start);
        Assert.Equal(new TimeOnly(11, 40), result.Slots[2].Start);
        Assert.Equal(new TimeOnly(13, 10), result.Slots[3].Start);

        // содержимое
        Assert.Equal("Прог", result.Slots[0].SubjectName);
        Assert.Equal("Петров И.С.", result.Slots[0].TeacherName);
        Assert.Equal("301", result.Slots[0].RoomNumber);
    }

    // === 2. Отмена слота 2 — остальные на своих местах, номера сохранены ===
    [Fact]
    public void Cancelled_middle_pair_keeps_other_slots_in_place()
    {
        var date = new DateOnly(2025, 10, 14);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithBasePair(DayOfWeek.Tuesday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .WithBasePair(DayOfWeek.Tuesday, 3, ScheduleContextBuilder.PhysId, ScheduleContextBuilder.IvanovId, ScheduleContextBuilder.SportZal)
            .WithBasePair(DayOfWeek.Tuesday, 4, ScheduleContextBuilder.MathId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithOverrideOut(ScheduleContextBuilder.Cancelled(date, 2, "Препод болеет"))
            .Build();

        var result = _resolver.Resolve(ctx);

        // 4 слота, второй — Cancelled
        Assert.Equal(4, result.Slots.Count);
        Assert.Equal(SlotStatus.Normal,    result.Slots[0].Status);
        Assert.Equal(SlotStatus.Cancelled, result.Slots[1].Status);
        Assert.Equal(SlotStatus.Normal,    result.Slots[2].Status);
        Assert.Equal(SlotStatus.Normal,    result.Slots[3].Status);

        // номера сохранены
        Assert.Equal(1, result.Slots[0].PairNumber);
        Assert.Equal(2, result.Slots[1].PairNumber);
        Assert.Equal(3, result.Slots[2].PairNumber);
        Assert.Equal(4, result.Slots[3].PairNumber);

        // времена НЕ сдвинуты — это ключевой кейс
        Assert.Equal(new TimeOnly(11, 40), result.Slots[2].Start);
        Assert.Equal(new TimeOnly(13, 10), result.Slots[3].Start);

        // причина отмены
        Assert.Equal("Препод болеет", result.Slots[1].Reason);
    }

    // === 3. Замена преподавателя ===
    [Fact]
    public void Substituted_teacher_replaces_teacher_only()
    {
        var date = new DateOnly(2025, 10, 14);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.MathId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithOverrideOut(ScheduleContextBuilder.SubstitutedTeacher(date, 1, ScheduleContextBuilder.SidorovId, "Замена"))
            .Build();

        var result = _resolver.Resolve(ctx);

        var slot = result.Slots[0];
        Assert.Equal(SlotStatus.Substituted, slot.Status);
        Assert.Equal("Матан", slot.SubjectName);        // предмет из базы
        Assert.Equal("Сидоров А.В.", slot.TeacherName); // препод из замены
        Assert.Equal("301", slot.RoomNumber);           // аудитория из базы
        Assert.Equal("Замена", slot.Reason);
    }

    // === 4. Отмена слота 2 + пустой слот 3 → 1, 2(Cancelled), 3(Empty), 4 ===
    [Fact]
    public void Empty_base_slot_returns_Empty_status()
    {
        var date = new DateOnly(2025, 10, 14);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            // слот 2 — пусто в базе
            .WithBasePair(DayOfWeek.Tuesday, 3, ScheduleContextBuilder.PhysId, ScheduleContextBuilder.IvanovId, ScheduleContextBuilder.SportZal)
            .WithBasePair(DayOfWeek.Tuesday, 4, ScheduleContextBuilder.MathId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.Equal(SlotStatus.Normal, result.Slots[0].Status);
        Assert.Equal(SlotStatus.Empty,  result.Slots[1].Status);
        Assert.Equal(SlotStatus.Normal, result.Slots[2].Status);
        Assert.Equal(SlotStatus.Normal, result.Slots[3].Status);

        Assert.Null(result.Slots[1].SubjectName);
        Assert.Null(result.Slots[1].TeacherName);
        Assert.Null(result.Slots[1].RoomNumber);
    }

    // === 5. Added на пустой слот — становится Added ===
    [Fact]
    public void Added_on_empty_slot_fills_it()
    {
        var date = new DateOnly(2025, 10, 14);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            // слот 2 — пусто
            .WithOverrideOut(ScheduleContextBuilder.Added(
                date, 2,
                ScheduleContextBuilder.MathId,
                ScheduleContextBuilder.SidorovId,
                ScheduleContextBuilder.Room302))
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.Equal(SlotStatus.Added, result.Slots[1].Status);
        Assert.Equal("Матан", result.Slots[1].SubjectName);
        Assert.Equal("Сидоров А.В.", result.Slots[1].TeacherName);
        Assert.Equal("302", result.Slots[1].RoomNumber);
    }

    // === 6. Added на ЗАНЯТЫЙ слот — игнорируется, остаётся базовая ===
    [Fact]
    public void Added_on_occupied_slot_is_ignored()
    {
        var date = new DateOnly(2025, 10, 14);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithOverrideOut(ScheduleContextBuilder.Added(
                date, 1,
                ScheduleContextBuilder.MathId,
                ScheduleContextBuilder.SidorovId,
                ScheduleContextBuilder.Room302))
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.Equal(SlotStatus.Normal, result.Slots[0].Status);
        Assert.Equal("Прог", result.Slots[0].SubjectName);
        Assert.Equal("Петров И.С.", result.Slots[0].TeacherName);
    }

    // === 7. Сокращённый день — другие времена, флаг IsShortened ===
    [Fact]
    public void Shortened_day_uses_shortened_bell_schedule()
    {
        var date = new DateOnly(2025, 10, 14);
        var shortened = ScheduleContextBuilder.ShortenedBellSchedule(Guid.NewGuid());

        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithBellSchedule(shortened)
            .WithCalendarDay(new CalendarDay
            {
                Id = Guid.NewGuid(),
                Date = date,
                IsShortened = true
            })
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithBasePair(DayOfWeek.Tuesday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.True(result.IsShortened);
        Assert.Equal("Сокращённое", result.BellScheduleName);

        // пары по часу
        Assert.Equal(new TimeOnly(8, 0),  result.Slots[0].Start);
        Assert.Equal(new TimeOnly(9, 0),  result.Slots[0].End);
        Assert.Equal(new TimeOnly(9, 5),  result.Slots[1].Start);
        Assert.Equal(new TimeOnly(10, 5), result.Slots[1].End);
    }

    [Fact]
    public void Saturday_replaced_with_Wednesday_uses_wednesday_schedule()
    {
        // суббота
        var saturday = new DateOnly(2025, 11, 15);
        Assert.Equal(DayOfWeek.Saturday, saturday.DayOfWeek);

        var ctx = new ScheduleContextBuilder()
            .OnDate(saturday.Year, saturday.Month, saturday.Day)
            .WithCalendarDay(new CalendarDay
            {
                Id = Guid.NewGuid(),
                Date = saturday,
                ReplacedWeekday = DayOfWeek.Wednesday
            })
            .WithBasePair(DayOfWeek.Wednesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .Build();

        var result = _resolver.Resolve(ctx);

        // эффективный день недели — среда
        Assert.Equal(DayOfWeek.Wednesday, result.EffectiveWeekday);

        // дата — суббота (не изменилась)
        Assert.Equal(new DateOnly(2025, 11, 15), result.Date);

        // слотов столько же, сколько пар в BellSchedule (4), но заполнен только первый
        Assert.Equal(4, result.Slots.Count);

        // слот 1 — пара со среды (Программирование, Петров)
        Assert.Equal(SlotStatus.Normal, result.Slots[0].Status);
        Assert.Equal("Прог", result.Slots[0].SubjectName);
        Assert.Equal("Петров И.С.", result.Slots[0].TeacherName);

        // слоты 2-4 пустые — в базовом расписании среды пар нет
        Assert.Equal(SlotStatus.Empty, result.Slots[1].Status);
        Assert.Equal(SlotStatus.Empty, result.Slots[2].Status);
        Assert.Equal(SlotStatus.Empty, result.Slots[3].Status);
    }

    // === 9. Праздник — пустой список слотов ===
    [Fact]
    public void Holiday_returns_empty_slots()
    {
        var date = new DateOnly(2026, 1, 1);
        var ctx = new ScheduleContextBuilder()
            .OnDate(date.Year, date.Month, date.Day)
            .WithCalendarDay(new CalendarDay
            {
                Id = Guid.NewGuid(),
                Date = date,
                IsHoliday = true
            })
            .WithBasePair(DayOfWeek.Thursday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.True(result.IsHoliday);
        Assert.Empty(result.Slots);
    }

    // === 10. Moved ИЗ слота — тут пусто, в другой день Substituted ===
    [Fact]
    public void Moved_out_empties_slot_in_source_day()
    {
        var monday = new DateOnly(2025, 10, 13);
        var tuesday = new DateOnly(2025, 10, 14);

        // Смотрим понедельник. Пара со слота 2 перенесена на вторник слот 1.
        var ctx = new ScheduleContextBuilder()
            .OnDate(monday.Year, monday.Month, monday.Day)
            .WithBasePair(DayOfWeek.Monday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithBasePair(DayOfWeek.Monday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .WithOverrideOut(ScheduleContextBuilder.MovedOut(monday, 2, tuesday, 1))
            .Build();

        var result = _resolver.Resolve(ctx);

        Assert.Equal(SlotStatus.Normal, result.Slots[0].Status);
        Assert.Equal(SlotStatus.Empty,  result.Slots[1].Status); // слот 2 пуст
    }

    // === 11. Moved В этот день — слот заполняется содержимым источника ===
    [Fact]
    public void Moved_in_fills_slot_from_source_day()
    {
        var monday = new DateOnly(2025, 10, 13);
        var tuesday = new DateOnly(2025, 10, 14);

        // Смотрим вторник. Пара из понедельника слот 2 перенесена сюда на слот 1.
        var ctx = new ScheduleContextBuilder()
            .OnDate(tuesday.Year, tuesday.Month, tuesday.Day)
            // базовая пары вторника в слоте 1 отсутствует — слот пуст
            .WithBasePair(DayOfWeek.Tuesday, 2, ScheduleContextBuilder.PhysId, ScheduleContextBuilder.IvanovId, ScheduleContextBuilder.SportZal)
            // но источник (понедельник слот 2) должен быть в BaseEntries
            .WithBasePair(DayOfWeek.Monday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .WithOverrideIn(ScheduleContextBuilder.MovedIn(monday, 2, tuesday, 1))
            .Build();

        var result = _resolver.Resolve(ctx);

        var slot1 = result.Slots[0];
        Assert.Equal(SlotStatus.Substituted, slot1.Status);
        Assert.Equal("Матан", slot1.SubjectName);
        Assert.Equal("Сидоров А.В.", slot1.TeacherName);
        Assert.Equal("302", slot1.RoomNumber);
        Assert.Contains("Перенесено", slot1.Reason);
    }

    // === 12. Moved В этот день на ЗАНЯТЫЙ слот — игнор ===
    [Fact]
    public void Moved_in_on_occupied_slot_is_ignored()
    {
        var monday = new DateOnly(2025, 10, 13);
        var tuesday = new DateOnly(2025, 10, 14);

        var ctx = new ScheduleContextBuilder()
            .OnDate(tuesday.Year, tuesday.Month, tuesday.Day)
            // слот 1 уже занят базовой парой
            .WithBasePair(DayOfWeek.Tuesday, 1, ScheduleContextBuilder.ProgId, ScheduleContextBuilder.PetrovId, ScheduleContextBuilder.Room301)
            .WithBasePair(DayOfWeek.Monday, 2, ScheduleContextBuilder.MathId, ScheduleContextBuilder.SidorovId, ScheduleContextBuilder.Room302)
            .WithOverrideIn(ScheduleContextBuilder.MovedIn(monday, 2, tuesday, 1))
            .Build();

        var result = _resolver.Resolve(ctx);

        // остаётся базовая, перенос игнорируется
        Assert.Equal(SlotStatus.Normal, result.Slots[0].Status);
        Assert.Equal("Прог", result.Slots[0].SubjectName);
    }
}