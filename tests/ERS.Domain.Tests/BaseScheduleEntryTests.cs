using ERS.Domain.Model;

namespace ERS.Domain.Tests;

public class BaseScheduleEntryTests
{
    [Fact]
    public void IsActiveOn_ReturnsTrue_WhenDateIsWithinRange()
    {
        var entry = new BaseScheduleEntry
        {
            ValidFrom = new DateOnly(2025, 9, 1),
            ValidTo = new DateOnly(2025, 12, 31)
        };

        var date = new DateOnly(2025, 10, 15);
        Assert.True(entry.IsActiveOn(date));
    }

    [Fact]
    public void IsActiveOn_ReturnsFalse_WhenDateIsBeforeRange()
    {
        var entry = new BaseScheduleEntry
        {
            ValidFrom = new DateOnly(2025, 9, 1),
            ValidTo = new DateOnly(2025, 12, 31)
        };

        var date = new DateOnly(2025, 8, 31);
        Assert.False(entry.IsActiveOn(date));
    }

    [Fact]
    public void IsActiveOn_ReturnsTrue_WhenValidToIsNull()
    {
        var entry = new BaseScheduleEntry
        {
            ValidFrom = new DateOnly(2025, 9, 1),
            ValidTo = null
        };

        var date = new DateOnly(2030, 1, 1);
        Assert.True(entry.IsActiveOn(date));
    }
}