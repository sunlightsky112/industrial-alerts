using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Config> Configs => Set<Config>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Config table
        modelBuilder.Entity<Config>().HasKey(c => c.Id);
        modelBuilder.Entity<Config>().HasData(new Config
        {
            Id = 1,
            TempMax = 75,
            HumidityMax = 60,
            UpdatedAt = new DateTimeOffset(2025, 01, 01, 0, 0, 0, TimeSpan.Zero)
        });

        // Alert table
        modelBuilder.Entity<Alert>().HasKey(a => a.Id);
        modelBuilder.Entity<Alert>().HasIndex(a => new { a.Status, a.CreatedAt });
    }
}
