using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PPKS_Parking_BE_API.Data;
using PPKS_Parking_BE_API.Models;

namespace PPKS_Parking_BE_API
{
    public class SeedScripts
    {
        // Metode za seedanje podataka u bazu
        async public static Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // Dodaj uloge ako ne postoje

            string[] roles = new[] { "ADMIN" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed ADMIN

            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "ADMIN");
            }
        }

        async public static Task SeedParkingDataAsync(ApplicationDbContext context)
        {
            // Seedanje parkinga

            if (context.Parkings.Any())
            {
                return;
            }

            var parking1 = new Parking
            {
                Name = "Parking 1",
                Blocks = new List<Block>
        {
            new Block
            {
                Name = "A",
                ParkingSpots = Enumerable.Range(1, 5).Select(i => new ParkingSpot
                {
                    Name = $"A{i}",
                    IsOccupied = false
                }).ToList()
            },
            new Block
            {
                Name = "B",
                ParkingSpots = Enumerable.Range(1, 3).Select(i => new ParkingSpot
                {
                    Name = $"B{i}",
                    IsOccupied = false
                }).ToList()
            }
        }
            };

            var parking2 = new Parking
            {
                Name = "Parking 2",
                Blocks = new List<Block>
        {
            new Block
            {
                Name = "A",
                ParkingSpots = Enumerable.Range(1, 4).Select(i => new ParkingSpot
                {
                    Name = $"A{i}",
                    IsOccupied = false
                }).ToList()
            }
        }
            };

            context.Parkings.AddRange(parking1, parking2);
            await context.SaveChangesAsync();
        }

        async public static Task SeedParkingLogsAsync(ApplicationDbContext context)
        {
            // Seed podataka logova

            if (!context.Parkings.Any())
                return;

            if (context.ParkingSpotUsageLogs.Any())
                return;

            var random = new Random();

            var startDate = DateTime.Now.Date.AddDays(-7);
            var endDate = DateTime.Now.Date;

            var allSpots = await context.ParkingSpots.ToListAsync();

            var logs = new List<ParkingSpotUsageLog>();

            foreach (var spot in allSpots)
            {
                var timestamp = startDate;

                while (timestamp < endDate)
                {
                    bool isOccupied = random.NextDouble() < 0.3;

                    logs.Add(new ParkingSpotUsageLog
                    {
                        ParkingSpotId = spot.Id,
                        IsOccupied = isOccupied,
                        Timestamp = timestamp
                    });

                    timestamp = timestamp.AddHours(1);
                }
            }
            context.ParkingSpotUsageLogs.AddRange(logs);
            await context.SaveChangesAsync();
        }
    }
}
