using Domain.Entities;
using Xunit;

namespace Domain.Tests;

public class ConfigTests
{
    [Theory]
    [InlineData(75, 60, true)]
    [InlineData(-5, 60, false)]
    [InlineData(75, 0, false)]
    public void Should_Validate_Thresholds(decimal tempMax, decimal humidityMax, bool expectedValid)
    {
        var config = new Config
        {
            TempMax = tempMax,
            HumidityMax = humidityMax,
            UpdatedAt = DateTime.UtcNow
        };

        var isValid = config.TempMax > 0 && config.HumidityMax > 0;

        Assert.Equal(expectedValid, isValid);
    }
}
