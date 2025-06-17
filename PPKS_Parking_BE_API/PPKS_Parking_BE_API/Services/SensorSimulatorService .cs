using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using PPKS_Parking_BE_API.Data;
using PPKS_Parking_BE_API.Models;

public class SensorSimulatorService : BackgroundService
{
    // Simulacija rada senzora na parkiralistu
    // U stvarnosti bi aplikacija primala podatke od senzora i updateala stanja parkirnih mjesta u bazi

    private readonly IServiceProvider _services;
    private readonly Random _random = new();

    public SensorSimulatorService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var allSpots = await dbContext.ParkingSpots.ToListAsync(stoppingToken);

            foreach (var spot in allSpots)
            {
                if (_random.NextDouble() < 0.5)
                {
                    spot.IsOccupied = !spot.IsOccupied;

                    var log = new ParkingSpotUsageLog
                    {
                        ParkingSpotId = spot.Id,
                        IsOccupied = spot.IsOccupied,
                        Timestamp = DateTime.UtcNow
                    };

                    dbContext.ParkingSpotUsageLogs.Add(log);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
