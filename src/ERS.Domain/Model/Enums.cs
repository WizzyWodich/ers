namespace ERS.Domain.Model;

public enum BellSectorType
{
    Pair,       // учебная пара
    Break,      // обычная перемена
    BigBreak    // большая перемена
}

public enum OverrideType
{
    Cancelled,    // пара отменена
    Substituted,  // замена полей (препод / аудитория / предмет)
    Moved,        // перенос на другой день / слот
    Added         // добавление в пустой слот
}

public enum OverrideStatus
{
    Draft,        // черновик, студент не видит
    Published,    // опубликовано, студент видит
    Cancelled     // сама замена отменена диспетчером
}