using Domain.Entities;
using Xunit;

namespace Domain.Tests;

public class AlertTests
{
    [Fact]
    public void Should_Create_Open_Alert_When_Value_Exceeds_Threshold()
    {
        // Arrange
        var threshold = 70m;
        var value = 80m;

        // Act
        var alert = new Alert
        {
            Type = AlertType.Temperature,
            Value = value,
            Threshold = threshold,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(AlertStatus.Open, alert.Status);
        Assert.True(alert.Value > alert.Threshold);
    }

    [Fact]
    public void Should_Acknowledge_Alert()
    {
        var alert = new Alert
        {
            Type = AlertType.Humidity,
            Value = 90m,
            Threshold = 60m,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        alert.Status = AlertStatus.Acknowledged;

        // Assert
        Assert.Equal(AlertStatus.Acknowledged, alert.Status);
    }
}
