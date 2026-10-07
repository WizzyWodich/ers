namespace ERS.Domain.Results;

public enum SlotStatus
{
    Normal,        // базовая пара без изменений
    Substituted,   // пара есть, что-то заменено
    Cancelled,     // пара отменена
    Added,         // пары не было в базовом, добавлена
    Empty          // слот пустой (окно в расписании)
}