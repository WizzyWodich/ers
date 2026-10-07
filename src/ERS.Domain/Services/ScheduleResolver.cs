using ERS.Domain.Model;
using ERS.Domain.Results;

namespace ERS.Domain.Services;

public sealed class ScheduleResolver
{
    public EffectiveSchedule Resolve(ResolveContext ctx)
    {
        var effectiveWeekday = ctx.CalendarDay?.ReplacedWeekday
                            ?? ctx.Date.DayOfWeek;

        if (ctx.CalendarDay?.IsHoliday == true)
        {
            return new EffectiveSchedule(
                ctx.Date,
                ctx.Group.DisplayName,
                ctx.BellSchedule.Name,
                IsHoliday: true,
                IsShortened: false,
                effectiveWeekday,
                Slots: []);
        }

        var pairSectors = ctx.BellSchedule.PairSectors;

        var baseByPair = ctx.BaseEntries
            .Where(e => e.Weekday == effectiveWeekday && e.IsActiveOn(ctx.Date))
            .ToDictionary(e => e.PairNumber);

        var overridesByPair = ctx.OverridesOut
            .GroupBy(o => o.PairNumber)
            .ToDictionary(g => g.Key, g => g.First());  

        var slots = new List<EffectiveSlot>(pairSectors.Count);

        for (var i = 0; i < pairSectors.Count; i++)
        {
            var pairNumber = i + 1;
            var sector = pairSectors[i];

            baseByPair.TryGetValue(pairNumber, out var baseEntry);
            overridesByPair.TryGetValue(pairNumber, out var ov);

            slots.Add(ResolveSlot(
                pairNumber, sector, baseEntry, ov, ctx));
        }

        // === Шаг 5. Переносы В этот день (Moved с NewDate == Date) ===
        foreach (var movedIn in ctx.OverridesIn)
        {
            if (movedIn.Type != OverrideType.Moved) continue;
            if (movedIn.NewPairNumber is not int targetPair) continue;
            if (targetPair < 1 || targetPair > pairSectors.Count) continue;

            var idx = targetPair - 1;

            // если слот уже занят — конфликт данных, не перетираем
            if (slots[idx].Status != SlotStatus.Empty)
                continue;

            // ищем, откуда перенесли (в исходном дне)
            var sourceWeekday = movedIn.TargetDate.DayOfWeek;
            var source = ctx.BaseEntries.FirstOrDefault(e =>
                e.Weekday == sourceWeekday &&
                e.PairNumber == movedIn.PairNumber);

            if (source is null)
                continue;

            var sector = pairSectors[idx];
            slots[idx] = new EffectiveSlot(
                targetPair,
                sector.Start,
                sector.End,
                SlotStatus.Substituted,
                ResolveSubject(source.SubjectId, ctx),
                ResolveTeacher(source.TeacherId, ctx),
                ResolveRoom(source.RoomId, ctx),
                $"Перенесено с {movedIn.TargetDate:dd.MM.yyyy}");
        }

        return new EffectiveSchedule(
            ctx.Date,
            ctx.Group.DisplayName,
            ctx.BellSchedule.Name,
            IsHoliday: false,
            IsShortened: ctx.CalendarDay?.IsShortened ?? false,
            effectiveWeekday,
            slots);
    }

    private static EffectiveSlot ResolveSlot(
        int pairNumber,
        BellSector sector,
        BaseScheduleEntry? baseEntry,
        ScheduleOverride? ov,
        ResolveContext ctx)
    {
        // --- Замены нет ---
        if (ov is null)
        {
            if (baseEntry is null)
                return Empty(pairNumber, sector);

            return new EffectiveSlot(
                pairNumber, sector.Start, sector.End,
                SlotStatus.Normal,
                ResolveSubject(baseEntry.SubjectId, ctx),
                ResolveTeacher(baseEntry.TeacherId, ctx),
                ResolveRoom(baseEntry.RoomId, ctx),
                Reason: null);
        }

        return ov.Type switch
        {
            OverrideType.Cancelled =>
                new EffectiveSlot(
                    pairNumber, sector.Start, sector.End,
                    SlotStatus.Cancelled,
                    null, null, null,
                    ov.Reason),

            OverrideType.Substituted =>
                ResolveSubstituted(pairNumber, sector, baseEntry, ov, ctx),

            OverrideType.Moved =>
                Empty(pairNumber, sector),   // тут пары больше нет

            OverrideType.Added =>
                baseEntry is null
                    ? new EffectiveSlot(
                        pairNumber, sector.Start, sector.End,
                        SlotStatus.Added,
                        ResolveSubject(ov.NewSubjectId, ctx),
                        ResolveTeacher(ov.NewTeacherId, ctx),
                        ResolveRoom(ov.NewRoomId, ctx),
                        ov.Reason)
                    // Added на занятый слот — ошибка данных, игнорим
                    : new EffectiveSlot(
                        pairNumber, sector.Start, sector.End,
                        SlotStatus.Normal,
                        ResolveSubject(baseEntry.SubjectId, ctx),
                        ResolveTeacher(baseEntry.TeacherId, ctx),
                        ResolveRoom(baseEntry.RoomId, ctx),
                        Reason: null),

            _ => Empty(pairNumber, sector)
        };
    }

    private static EffectiveSlot ResolveSubstituted(
        int pairNumber,
        BellSector sector,
        BaseScheduleEntry? baseEntry,
        ScheduleOverride ov,
        ResolveContext ctx)
    {
        var subjectId = ov.NewSubjectId ?? baseEntry?.SubjectId;
        var teacherId = ov.NewTeacherId ?? baseEntry?.TeacherId;
        var roomId    = ov.NewRoomId    ?? baseEntry?.RoomId;

        // Если и в базовом ничего нет, и замена пустая — это окно
        if (subjectId is null && teacherId is null && roomId is null)
            return Empty(pairNumber, sector);

        return new EffectiveSlot(
            pairNumber, sector.Start, sector.End,
            SlotStatus.Substituted,
            ResolveSubject(subjectId, ctx),
            ResolveTeacher(teacherId, ctx),
            ResolveRoom(roomId, ctx),
            ov.Reason);
    }

    private static EffectiveSlot Empty(int pairNumber, BellSector sector) =>
        new(pairNumber, sector.Start, sector.End,
            SlotStatus.Empty, null, null, null, null);

    private static string? ResolveSubject(Guid? id, ResolveContext ctx) =>
        id is Guid g && ctx.Subjects.TryGetValue(g, out var s) ? s.ShortName : null;

    private static string? ResolveTeacher(Guid? id, ResolveContext ctx) =>
        id is Guid g && ctx.Teachers.TryGetValue(g, out var t) ? t.ShortName : null;

    private static string? ResolveRoom(Guid? id, ResolveContext ctx) =>
        id is Guid g && ctx.Rooms.TryGetValue(g, out var r) ? r.Number : null;
}