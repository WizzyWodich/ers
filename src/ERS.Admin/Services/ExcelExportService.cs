using ClosedXML.Excel;

namespace ERS.Admin.Services;

public sealed class ExcelExportService
{
    public sealed record OverrideRow(
        string Group,
        DateOnly Date,
        string Pairs,
        string Type,           // "Normal" | "Cancelled" | "Substituted" | "Moved" | "Added" | "FreeDay"
        string? ChangeType,    // "Преподаватель" | "Предмет" | "Аудитория" (для Substituted)
        string Subject,        // предмет пары (базовый или итоговый)
        string? Teacher,       // препод пары (базовый или итоговый)
        string? Old,           // что было (для замены/переноса)
        string? New,           // что стало (для замены/переноса/добавления)
        string? Reason);

    private static readonly string[] Types = { "Отмена", "Замена", "Перенос", "Добавление", "Свободный день" };
    private static readonly string[] ChangeTypes = { "Преподаватель", "Предмет", "Аудитория" };
    private static readonly string[] KnownGroups = { "К-21", "К-22", "К-41", "Б-31", "Б-32", "П-41", "Аг-11", "Аг-21", "Д-11", "Д-21", "Е-31", "Е-32" };
    private static readonly string[] KnownTeachers = { "Петров И.С.", "Сидоров А.В.", "Иванов П.П.", "Кузнецова М.И.", "Смирнов Д.А.", "Тітянкин С.С." };
    private static readonly string[] KnownSubjects = { "Программирование", "Математика", "Физкультура", "История", "ОБЖ", "Практика", "Опер. системы" };
    private static readonly string[] KnownRooms = { "301", "302", "305", "Спортзал" };

    private const string RefsSheetName = "Справочники";
    private const string FontName = "Times New Roman";
    private const int PrintFontSize = 14;

    public byte[] ExportOverrides(IEnumerable<OverrideRow> overrides, string title)
    {
        var list = overrides.ToList();

        using var workbook = new XLWorkbook();

        BuildRefsSheet(workbook);
        BuildDataSheet(workbook, list, title);
        BuildPrintSheet(workbook, list, title);

        workbook.Worksheet(RefsSheetName).Hide();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public byte[] ExportTemplate()
    {
        using var workbook = new XLWorkbook();

        var examples = new List<OverrideRow>
        {
            // Замена препода
            new("К-41", new DateOnly(2025, 4, 21), "3-5", "Substituted", "Преподаватель",
                "Практика", "Петров И.С.", "Тітянкин С.С.", "Петров И.С.", "Курсы повышения"),
            // Замена предмета
            new("К-31", new DateOnly(2025, 4, 21), "1", "Substituted", "Предмет",
                "Опер. системы", "Хамцов А.В.", "Опер. системы", "Практика", "По приказу"),
            // Отмена
            new("Б-31", new DateOnly(2025, 4, 22), "2", "Cancelled", null,
                "Математика", "Сидоров А.В.", null, null, "Аудитория занята"),
        };

        BuildRefsSheet(workbook);
        BuildDataSheet(workbook, examples, "Шаблон замен", isTemplate: true);
        BuildPrintSheet(workbook, examples, "Пример (удалите перед заполнением)");

        workbook.Worksheet(RefsSheetName).Hide();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    // ═══════════════════════════════════════════════════════════
    // ЛИСТ «СПРАВОЧНИКИ»
    // ═══════════════════════════════════════════════════════════
    private void BuildRefsSheet(XLWorkbook workbook)
    {
        var sheet = workbook.Worksheets.Add(RefsSheetName);

        var columns = new (string Header, string[] Values)[]
        {
            ("Группы",         KnownGroups),
            ("Преподаватели",  KnownTeachers),
            ("Предметы",       KnownSubjects),
            ("Аудитории",      KnownRooms),
            ("Типы",           Types),
            ("Что меняем",     ChangeTypes),
        };

        for (int c = 0; c < columns.Length; c++)
        {
            var (header, values) = columns[c];
            var col = c + 1;

            var h = sheet.Cell(1, col);
            h.Value = header;
            h.Style.Font.Bold = true;
            h.Style.Font.FontName = FontName;
            h.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

            for (int i = 0; i < values.Length; i++)
            {
                var cell = sheet.Cell(i + 2, col);
                cell.Value = values[i];
                cell.Style.Font.FontName = FontName;
            }

            sheet.Column(col).Width = 20;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // ЛИСТ «ДАННЫЕ»
    // ═══════════════════════════════════════════════════════════
    private void BuildDataSheet(XLWorkbook workbook, List<OverrideRow> overrides, string title, bool isTemplate = false)
    {
        var sheet = workbook.Worksheets.Add("Данные");
        var headers = new[] { "Группа", "Дата", "Пары", "Тип", "Что меняем", "Предмет", "Преподаватель", "Старое", "Новое", "Причина" };

        // Заголовок
        var titleCell = sheet.Cell(1, 1);
        titleCell.Value = title;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 14;
        titleCell.Style.Font.FontName = FontName;
        titleCell.Style.Font.FontColor = XLColor.FromHtml("#0F172A");
        sheet.Range(1, 1, 1, headers.Length).Merge();
        sheet.Row(1).Height = 28;

        // Инструкция
        var instr = sheet.Cell(2, 1);
        instr.Value = "Одна строка = одна замена. Тип, группы и предметы выбираются из выпадающих списков.";
        instr.Style.Font.Italic = true;
        instr.Style.Font.FontSize = 10;
        instr.Style.Font.FontName = FontName;
        instr.Style.Font.FontColor = XLColor.FromHtml("#64748B");
        sheet.Range(2, 1, 2, headers.Length).Merge();
        sheet.Row(2).Height = 18;

        // Шапка
        int headerRow = 4;
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(headerRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 10;
            cell.Style.Font.FontName = FontName;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.BottomBorderColor = XLColor.FromHtml("#94A3B8");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        sheet.Row(headerRow).Height = 26;

        // Данные
        int row = headerRow + 1;
        foreach (var o in overrides)
        {
            sheet.Cell(row, 1).Value = o.Group;
            sheet.Cell(row, 2).Value = o.Date.ToDateTime(TimeOnly.MinValue);
            sheet.Cell(row, 2).Style.DateFormat.Format = "dd.MM.yyyy";
            sheet.Cell(row, 3).Value = o.Pairs;
            sheet.Cell(row, 4).Value = MapTypeToRu(o.Type);
            sheet.Cell(row, 5).Value = o.ChangeType ?? "";
            sheet.Cell(row, 6).Value = o.Subject;
            sheet.Cell(row, 7).Value = o.Teacher ?? "";
            sheet.Cell(row, 8).Value = o.Old ?? "";
            sheet.Cell(row, 9).Value = o.New ?? "";
            sheet.Cell(row, 10).Value = o.Reason ?? "";

            var range = sheet.Range(row, 1, row, headers.Length);
            range.Style.Font.FontSize = 10;
            range.Style.Font.FontName = FontName;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.FromHtml("#E2E8F0");

            sheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            row++;
        }

        if (isTemplate)
        {
            for (int extra = 0; extra < 100; extra++)
            {
                var range = sheet.Range(row, 1, row, headers.Length);
                range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                range.Style.Border.BottomBorderColor = XLColor.FromHtml("#E2E8F0");
                row++;
            }
        }
        int lastRow = row - 1;

        // Валидации
        AddListValidation(sheet, $"A{headerRow + 1}:A{lastRow}", "A");
        AddListValidation(sheet, $"D{headerRow + 1}:D{lastRow}", "E");
        AddListValidation(sheet, $"E{headerRow + 1}:E{lastRow}", "F");
        AddListValidation(sheet, $"F{headerRow + 1}:F{lastRow}", "C");
        AddListValidation(sheet, $"G{headerRow + 1}:G{lastRow}", "B");

        // Ширины
        sheet.Column(1).Width = 10;
        sheet.Column(2).Width = 13;
        sheet.Column(3).Width = 8;
        sheet.Column(4).Width = 14;
        sheet.Column(5).Width = 16;
        sheet.Column(6).Width = 20;
        sheet.Column(7).Width = 20;
        sheet.Column(8).Width = 20;
        sheet.Column(9).Width = 20;
        sheet.Column(10).Width = 28;

        sheet.SheetView.FreezeRows(headerRow);
        sheet.Range(headerRow, 1, lastRow, headers.Length).SetAutoFilter();

        sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        sheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        sheet.PageSetup.PagesWide = 1;
        sheet.PageSetup.PagesTall = 0;
        sheet.PageSetup.Margins.SetLeft(0.4).SetRight(0.4).SetTop(0.5).SetBottom(0.5);
        sheet.PageSetup.SetRowsToRepeatAtTop(headerRow, headerRow);

        sheet.ShowGridLines = false;
    }

    // ═══════════════════════════════════════════════════════════
    // ЛИСТ «НА ПЕЧАТЬ» — формат как в документе
    // ═══════════════════════════════════════════════════════════
    private void BuildPrintSheet(XLWorkbook workbook, List<OverrideRow> overrides, string title)
    {
        var sheet = workbook.Worksheets.Add("На печать");

        // Заголовок
        var titleCell = sheet.Cell(1, 1);
        titleCell.Value = title;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 18;
        titleCell.Style.Font.FontName = FontName;
        titleCell.Style.Font.FontColor = XLColor.FromHtml("#0F172A");
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Range(1, 1, 1, 4).Merge();
        sheet.Row(1).Height = 38;

        var subtitle = sheet.Cell(2, 1);
        subtitle.Value = "Изменения в расписании занятий";
        subtitle.Style.Font.FontSize = 12;
        subtitle.Style.Font.FontName = FontName;
        subtitle.Style.Font.FontColor = XLColor.FromHtml("#64748B");
        subtitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        sheet.Range(2, 1, 2, 4).Merge();
        sheet.Row(2).Height = 20;

        int row = 4;

        var grouped = overrides
            .GroupBy(o => o.Date)
            .OrderBy(g => g.Key);

        foreach (var dayGroup in grouped)
        {
            // Заголовок дня
            var dayHeader = sheet.Cell(row, 1);
            dayHeader.Value = $"{dayGroup.Key:dd MMMM yyyy}, {RussianDayOfWeek(dayGroup.Key.DayOfWeek)}";
            dayHeader.Style.Font.Bold = true;
            dayHeader.Style.Font.FontSize = 16;
            dayHeader.Style.Font.FontName = FontName;
            dayHeader.Style.Font.FontColor = XLColor.FromHtml("#1E40AF");
            dayHeader.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF");
            dayHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dayHeader.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            sheet.Range(row, 1, row, 4).Merge();
            sheet.Row(row).Height = 30;
            row++;

            foreach (var o in dayGroup.OrderBy(x => x.Group))
            {
                // Колонка 1: Группа · пары
                var groupCell = sheet.Cell(row, 1);
                groupCell.Value = $"{o.Group}-{o.Pairs}п.";
                groupCell.Style.Font.Bold = true;
                groupCell.Style.Font.FontSize = PrintFontSize;
                groupCell.Style.Font.FontName = FontName;
                groupCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                groupCell.Style.Alignment.Indent = 1;

                // Колонка 2-3: содержимое (объединяем для длинных строк)
                var contentCell = sheet.Cell(row, 2);
                contentCell.Value = BuildContentText(o);
                contentCell.Style.Font.Bold = true;
                contentCell.Style.Font.FontSize = PrintFontSize;
                contentCell.Style.Font.FontName = FontName;
                contentCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                contentCell.Style.Alignment.WrapText = true;
                contentCell.Style.Alignment.Indent = 1;
                sheet.Range(row, 2, row, 4).Merge();

                // Границы строки
                var range = sheet.Range(row, 1, row, 4);
                range.Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
                range.Style.Border.BottomBorderColor = XLColor.FromHtml("#CBD5E1");

                sheet.Row(row).Height = 28;
                row++;

                // Причина — отдельная строка курсивом, если есть
                if (!string.IsNullOrEmpty(o.Reason))
                {
                    var reasonCell = sheet.Cell(row, 2);
                    reasonCell.Value = o.Reason;
                    reasonCell.Style.Font.Italic = true;
                    reasonCell.Style.Font.FontSize = 11;
                    reasonCell.Style.Font.FontName = FontName;
                    reasonCell.Style.Font.FontColor = XLColor.FromHtml("#64748B");
                    reasonCell.Style.Alignment.Indent = 2;
                    reasonCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    sheet.Range(row, 2, row, 4).Merge();
                    sheet.Row(row).Height = 20;
                    row++;
                }
            }

            sheet.Row(row).Height = 10;
            row++;
        }

        if (!overrides.Any())
        {
            var empty = sheet.Cell(row, 1);
            empty.Value = "Замен нет";
            empty.Style.Font.Italic = true;
            empty.Style.Font.FontName = FontName;
            empty.Style.Font.FontSize = PrintFontSize;
            empty.Style.Font.FontColor = XLColor.FromHtml("#94A3B8");
            empty.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Range(row, 1, row, 4).Merge();
        }

        // Ширины
        sheet.Column(1).Width = 18;   // Группа-пары
        sheet.Column(2).Width = 32;   // Содержимое
        sheet.Column(3).Width = 20;
        sheet.Column(4).Width = 20;

        sheet.PageSetup.PageOrientation = XLPageOrientation.Portrait;
        sheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        sheet.PageSetup.PagesWide = 1;
        sheet.PageSetup.PagesTall = 0;
        sheet.PageSetup.Margins.SetLeft(0.5).SetRight(0.5).SetTop(0.6).SetBottom(0.6).SetHeader(0.3).SetFooter(0.3);
        sheet.PageSetup.CenterHorizontally = true;

        sheet.PageSetup.Footer.Center.AddText("Стр. &P из &N");
        sheet.PageSetup.Footer.Left.AddText($"Сформировано {DateTime.Now:dd.MM.yyyy HH:mm}");

        sheet.ShowGridLines = false;
    }

    // ═══════════════════════════════════════════════════════════
    // Формат содержимого для печати — как в документе
    // ═══════════════════════════════════════════════════════════
    private static string BuildContentText(OverrideRow o)
    {
        var subject = o.Subject;
        var teacher = o.Teacher;

        return o.Type switch
        {
            // Обычная пара (на всякий случай)
            "Normal" => $"{subject}, {teacher}",

            // Замена — показываем что было и что стало
            "Substituted" => o.ChangeType switch
            {
                "Преподаватель" => $"{subject}, {o.Old} → {o.New}",
                "Предмет"       => $"{o.Old} → {o.New}, {teacher}",
                "Аудитория"     => $"{subject}, {teacher}, ауд. {o.Old} → {o.New}",
                _               => $"{subject}, {teacher}"
            },

            // Отмена — просто отменено с указанием предмета
            "Cancelled" => $"ОТМЕНЕНО · {subject}, {teacher}",

            // Перенос
            "Moved" => $"{subject}, {teacher} → на {o.New}",

            // Добавление
            "Added" => $"{subject}, {teacher}",

            // Свободный день
            "FreeDay" => "СВОБОДНЫЙ ДЕНЬ",

            _ => $"{subject}, {teacher}"
        };
    }

    private static string MapTypeToRu(string type) => type switch
    {
        "Cancelled"   => "Отмена",
        "Substituted" => "Замена",
        "Moved"       => "Перенос",
        "Added"       => "Добавление",
        "FreeDay"     => "Свободный день",
        "Normal"      => "Обычная",
        _ => type
    };

    private static string RussianDayOfWeek(DayOfWeek dow) => dow switch
    {
        DayOfWeek.Monday => "понедельник",
        DayOfWeek.Tuesday => "вторник",
        DayOfWeek.Wednesday => "среда",
        DayOfWeek.Thursday => "четверг",
        DayOfWeek.Friday => "пятница",
        DayOfWeek.Saturday => "суббота",
        DayOfWeek.Sunday => "воскресенье",
        _ => ""
    };

    private static void AddListValidation(IXLWorksheet sheet, string range, string refColumn)
    {
        var formula = $"={RefsSheetName}!${refColumn}$2:${refColumn}$1000";

        var validation = sheet.Range(range).CreateDataValidation();
        validation.List(formula, true);
        validation.IgnoreBlanks = true;
        validation.ShowErrorMessage = false;
        validation.ShowInputMessage = false;
    }
}