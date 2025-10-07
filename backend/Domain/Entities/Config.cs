
namespace Domain.Entities;

public class Config
{
    public int Id { get; set; } = 1; // single row
    public decimal TempMax { get; set; }
    public decimal HumidityMax { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
