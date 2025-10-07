
namespace Domain.Entities;

public enum AlertType { Temperature, Humidity }
public enum AlertStatus { Open, Acknowledged }

public class Alert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public AlertType Type { get; set; }
    public decimal Value { get; set; }
    public decimal Threshold { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public AlertStatus Status { get; set; } = AlertStatus.Open;
}
