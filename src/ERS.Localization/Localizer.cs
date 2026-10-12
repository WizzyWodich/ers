namespace ERS.Localization;

public sealed class Localizer
{
    public Language Current { get; private set; } = Language.En;

    /// <summary>
    /// Событие смены языка. Компоненты подписываются и обновляются.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Получить строку по ключу.
    /// </summary>
    public string this[string key]
    {
        get
        {
            if (Strings.All.TryGetValue(Current, out var dict) && dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return key;  
        }
    }

    /// <summary>
    /// Форматирование строки с параметрами: Format("break.short", 10) → "Break · 10 min"
    /// </summary>
    public string Format(string key, params object[] args)
    {
        var template = this[key];
        try { return string.Format(template, args); }
        catch { return template; }
    }

    /// <summary>
    /// Полное название дня недели.
    /// </summary>
    public string DayOfWeekFull(DayOfWeek d) => this[$"day.{d.ToString().ToLowerInvariant()}"];

    /// <summary>
    /// Короткое название дня недели (для календаря).
    /// </summary>
    public string DayOfWeekShort(DayOfWeek d) => this[$"day.short.{d.ToString().ToLowerInvariant()}"];

    /// <summary>
    /// Название месяца в родительном падеже («14 жовтня», «14 October»).
    /// </summary>
    public string MonthName(int month) => this[$"month.{month}"];

    public void SetLanguage(Language lang)
    {
        if (Current == lang) return;
        Current = lang;
        OnChange?.Invoke();
    }

    public static string GetLanguageCode(Language lang) => lang switch
    {
        Language.En => "en",
        Language.Uk => "uk",
        _ => "en"
    };

    public static Language ParseLanguage(string? code) => code switch
    {
        "uk" => Language.Uk,
        _ => Language.En
    };
}