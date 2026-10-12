namespace ERS.Localization;

public static class Strings
{
    public static readonly Dictionary<Language, Dictionary<string, string>> All = new()
    {
        [Language.En] = new()
        {
            ["nav.schedule"] = "Schedule",
            ["nav.weekDesk"] = "Week desk",
            ["nav.overrides"] = "Overrides",

            ["layout.brand"] = "ERS Admin",
            ["layout.brandSubtitle"] = "Dispatcher panel",
            ["layout.theme"] = "Theme",
            ["layout.dispatcher"] = "Dispatcher",
            ["layout.comingSoon"] = "Coming soon",

            ["common.cancel"] = "Cancel",
            ["common.save"] = "Save",
            ["common.create"] = "Create",
            ["common.delete"] = "Delete",
            ["common.edit"] = "Edit",
            ["common.close"] = "Close",
            ["common.search"] = "Search",
            ["common.publish"] = "Publish",
            ["common.publishAll"] = "Publish all",
            ["common.new"] = "New",
            ["common.loading"] = "Loading...",
            ["common.empty"] = "Nothing here yet",
            ["common.inDevelopment"] = "In development",
            ["common.addGroup"] = "Add group",

            ["home.title"] = "Schedule",
            ["home.subtitle"] = "Select a group to view the schedule",
            ["home.searchPlaceholder"] = "Search groups...",
            ["home.addGroup"] = "Add group",
            ["home.groups"] = "Groups",
            ["home.noResults"] = "No groups found",

            ["layout.language"] = "Language",
            ["lang.en"] = "EN",
            ["lang.uk"] = "UK",

            // ── Days ──
            ["day.monday"] = "Monday",
            ["day.tuesday"] = "Tuesday",
            ["day.wednesday"] = "Wednesday",
            ["day.thursday"] = "Thursday",
            ["day.friday"] = "Friday",
            ["day.saturday"] = "Saturday",
            ["day.sunday"] = "Sunday",

            ["day.short.monday"] = "Mon",
            ["day.short.tuesday"] = "Tue",
            ["day.short.wednesday"] = "Wed",
            ["day.short.thursday"] = "Thu",
            ["day.short.friday"] = "Fri",
            ["day.short.saturday"] = "Sat",
            ["day.short.sunday"] = "Sun",

            // ── Months (genitive) ──
            ["month.1"] = "January",
            ["month.2"] = "February",
            ["month.3"] = "March",
            ["month.4"] = "April",
            ["month.5"] = "May",
            ["month.6"] = "June",
            ["month.7"] = "July",
            ["month.8"] = "August",
            ["month.9"] = "September",
            ["month.10"] = "October",
            ["month.11"] = "November",
            ["month.12"] = "December",

            // ── Group page ──
            ["group.about"] = "About group",
            ["group.specialty"] = "Specialty",
            ["group.course"] = "Course",
            ["group.students"] = "Students",
            ["group.today"] = "Today",
            ["group.pairs"] = "Pairs",
            ["group.start"] = "Start",
            ["group.end"] = "End",
            ["group.substitutions"] = "Substitutions",
            ["group.cancelled"] = "Cancelled",
            ["group.curator"] = "Curator",
            ["group.createOverride"] = "Create override",
            ["group.print"] = "Print",

            // ── Date navigator ──
            ["nav.prev"] = "Previous",
            ["nav.next"] = "Next",
            ["nav.today"] = "Today",

            // ── Schedule table ──
            ["table.pair"] = "Pair",
            ["table.subject"] = "Subject",
            ["table.teacher"] = "Teacher",
            ["table.room"] = "Room",
            ["table.action"] = "Action",
            ["table.change"] = "Change",
            ["table.return"] = "Return",
            ["table.draft"] = "Draft",
            ["table.substitution"] = "Substitution",
            ["table.cancelled"] = "Cancelled",
            ["table.added"] = "Added",

            // ── Breaks ──
            ["break.short"] = "Break · {0} min",
            ["break.long"] = "Long break · {0} min",

            // ── Group switcher ──
            ["switcher.searchGroups"] = "Search groups...",
            ["switcher.noGroups"] = "No groups found",

            // ── Publish ──
            ["publish.button"] = "Publish ({0})",
            ["publish.title"] = "Publish {0} changes",
        },

        [Language.Uk] = new()
        {
            ["nav.schedule"] = "Розклад",
            ["nav.weekDesk"] = "Робочий стіл тижня",
            ["nav.overrides"] = "Заміни",

            ["layout.brand"] = "ERS Admin",
            ["layout.brandSubtitle"] = "Панель диспетчера",
            ["layout.theme"] = "Тема",
            ["layout.dispatcher"] = "Диспетчер",
            ["layout.comingSoon"] = "В розробці",

            ["common.cancel"] = "Скасувати",
            ["common.save"] = "Зберегти",
            ["common.create"] = "Створити",
            ["common.delete"] = "Видалити",
            ["common.edit"] = "Змінити",
            ["common.close"] = "Закрити",
            ["common.search"] = "Пошук",
            ["common.publish"] = "Опублікувати",
            ["common.publishAll"] = "Опублікувати все",
            ["common.new"] = "Новий",
            ["common.loading"] = "Завантаження...",
            ["common.empty"] = "Поки нічого немає",
            ["common.inDevelopment"] = "В розробці",
            ["common.addGroup"] = "Додати групу",

            ["home.title"] = "Розклад",
            ["home.subtitle"] = "Виберіть групу, щоб переглянути розклад",
            ["home.searchPlaceholder"] = "Пошук груп...",
            ["home.addGroup"] = "Додати групу",
            ["home.groups"] = "Групи",
            ["home.noResults"] = "Групи не знайдено",

            ["layout.language"] = "Мова",
            ["lang.en"] = "EN",
            ["lang.uk"] = "UK",

            // ── Days ──
            ["day.monday"] = "Понеділок",
            ["day.tuesday"] = "Вівторок",
            ["day.wednesday"] = "Середа",
            ["day.thursday"] = "Четвер",
            ["day.friday"] = "П'ятниця",
            ["day.saturday"] = "Субота",
            ["day.sunday"] = "Неділя",

            ["day.short.monday"] = "Пн",
            ["day.short.tuesday"] = "Вт",
            ["day.short.wednesday"] = "Ср",
            ["day.short.thursday"] = "Чт",
            ["day.short.friday"] = "Пт",
            ["day.short.saturday"] = "Сб",
            ["day.short.sunday"] = "Нд",

            // ── Months ──
            ["month.1"] = "січня",
            ["month.2"] = "лютого",
            ["month.3"] = "березня",
            ["month.4"] = "квітня",
            ["month.5"] = "травня",
            ["month.6"] = "червня",
            ["month.7"] = "липня",
            ["month.8"] = "серпня",
            ["month.9"] = "вересня",
            ["month.10"] = "жовтня",
            ["month.11"] = "листопада",
            ["month.12"] = "грудня",

            // ── Group page ──
            ["group.about"] = "Про групу",
            ["group.specialty"] = "Спеціальність",
            ["group.course"] = "Курс",
            ["group.students"] = "Студентів",
            ["group.today"] = "Сьогодні",
            ["group.pairs"] = "Пар",
            ["group.start"] = "Початок",
            ["group.end"] = "Кінець",
            ["group.substitutions"] = "Замін",
            ["group.cancelled"] = "Скасовано",
            ["group.curator"] = "Куратор",
            ["group.createOverride"] = "Створити заміну",
            ["group.print"] = "Друк",

            // ── Date navigator ──
            ["nav.prev"] = "Попередній",
            ["nav.next"] = "Наступний",
            ["nav.today"] = "Сьогодні",

            // ── Schedule table ──
            ["table.pair"] = "Пара",
            ["table.subject"] = "Дисципліна",
            ["table.teacher"] = "Викладач",
            ["table.room"] = "Аудиторія",
            ["table.action"] = "Дія",
            ["table.change"] = "Змінити",
            ["table.return"] = "Повернути",
            ["table.draft"] = "Чернетка",
            ["table.substitution"] = "Заміна",
            ["table.cancelled"] = "Скасовано",
            ["table.added"] = "Додано",

            // ── Breaks ──
            ["break.short"] = "Перерва · {0} хв",
            ["break.long"] = "Велика перерва · {0} хв",

            // ── Group switcher ──
            ["switcher.searchGroups"] = "Пошук груп...",
            ["switcher.noGroups"] = "Групи не знайдено",

            // ── Publish ──
            ["publish.button"] = "Опублікувати ({0})",
            ["publish.title"] = "Опублікувати {0} змін",
        }
    };
}