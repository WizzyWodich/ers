using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using ERS.Localization;

namespace ERS.Web.Pages;

[Authorize]
public partial class Schedule
{
    [Inject] private Localizer L { get; set; } = default!;

    private DateOnly CurrentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    private bool showMonth;

    private void ToggleCalendar(bool month) => showMonth = month;
    private void SelectDate(DateOnly date) => CurrentDate = date;
    private void GoToToday() => CurrentDate = DateOnly.FromDateTime(DateTime.Today);

    public sealed record MockPair(
        string Start, string End, 
        string Subject, string Teacher, string? OldTeacher, 
        string Room, string Color, string Status, 
        int BreakAfterMinutes);

    private List<MockPair> PairsForDate => CurrentDate.DayOfWeek switch
    {
        DayOfWeek.Monday => new()
        {
            new("08:30", "09:50", "Вища математика", "Петров І.С.", null, "301", "blue", "Normal", 20),
            new("10:10", "11:30", "Фізика", "Іванов П.П.", null, "Лаб. 12", "emerald", "Normal", 40),
            new("12:10", "13:30", "Укр. мова", "Кузнєцова М.І.", null, "305", "violet", "Normal", 0),
        },
        DayOfWeek.Tuesday => new()
        {
            new("08:30", "09:50", "Програмування", "Петров І.С.", null, "301", "blue", "Normal", 20),
            new("10:10", "11:30", "Математика", "Сидоров О.В.", "Петров І.С.", "302", "amber", "Substituted", 40),
            new("12:10", "13:30", "Історія", "Кузнєцова М.І.", null, "305", "slate", "Cancelled", 0),
        },
        DayOfWeek.Wednesday => new()
        {
            new("08:30", "09:50", "Англійська", "Сміт Дж.", null, "205", "pink", "Normal", 20),
            new("10:10", "11:30", "Програмування", "Петров І.С.", null, "301", "blue", "Normal", 0),
        },
        DayOfWeek.Thursday => new(),
        DayOfWeek.Friday => new()
        {
            new("08:30", "09:50", "Фізкультура", "Іванов П.П.", null, "Спортзал", "emerald", "Normal", 0),
        },
        _ => new()
    };
}