using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public class SensorSimulator : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SensorSimulator> _logger;
    private readonly Random _rand = new();

    public SensorSimulator(IServiceProvider services, ILogger<SensorSimulator> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cfg = await db.Configs.FirstOrDefaultAsync(c => c.Id == 1, stoppingToken);
            if (cfg is null)
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            // Simulate readings
            var temp = 60 + _rand.NextDouble() * 30;     // 60–90
            var humidity = 40 + _rand.NextDouble() * 40; // 40–80

            // Check thresholds
            if (temp > (double)cfg.TempMax)
            {
                db.Alerts.Add(new Alert
                {
                    Type = AlertType.Temperature,
                    Value = (decimal)temp,
                    Threshold = cfg.TempMax
                });
                _logger.LogWarning("Temperature alert triggered: {Temp}", temp);
            }

            if (humidity > (double)cfg.HumidityMax)
            {
                db.Alerts.Add(new Alert
                {
                    Type = AlertType.Humidity,
                    Value = (decimal)humidity,
                    Threshold = cfg.HumidityMax
                });
                _logger.LogWarning("Humidity alert triggered: {Humidity}", humidity);
            }

            await db.SaveChangesAsync(stoppingToken);

            // Wait 3–5 seconds before next reading
            var delaySeconds = 3 + _rand.Next(0, 3); // 3, 4, or 5
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
        }
    }
}
