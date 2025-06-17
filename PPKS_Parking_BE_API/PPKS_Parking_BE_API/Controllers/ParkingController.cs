using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPKS_Parking_BE_API.Data;
using PPKS_Parking_BE_API.Models;

namespace PPKS_Parking_BE_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parking>>> GetAll()
        {
            try
            {
                var parkings = await _context.Parkings
                    .Include(p => p.Blocks)
                        .ThenInclude(b => b.ParkingSpots)
                    .ToListAsync();

                return Ok(parkings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska prilikom dohvata parkinga: " + ex.Message);
                return StatusCode(500, "Greška na serveru");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var parking = await _context.Parkings
                    .Include(p => p.Blocks)
                        .ThenInclude(b => b.ParkingSpots)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (parking == null)
                    return NotFound("Parking nije pronađen.");

                var allSpots = parking.Blocks.SelectMany(b => b.ParkingSpots).ToList();
                var totalSpots = allSpots.Count;
                var freeSpots = parking.FreeSpotsCount;
                var occupiedSpots = totalSpots - freeSpots;

                var spotIds = allSpots.Select(s => s.Id).ToList();

                var logs = await _context.ParkingSpotUsageLogs
                    .Where(log => spotIds.Contains(log.ParkingSpotId))
                    .ToListAsync();

                // Svi dani u tjednu
                var allDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

                var weeklyStats = allDays.Select(day =>
                {
                    var logsForDay = logs.Where(l => l.Timestamp.DayOfWeek == day).ToList();
                    var total = logsForDay.Count;
                    return new
                    {
                        Day = day.ToString(),
                        TotalRecords = total,
                        AvgOccupiedPercent = total > 0 ? logsForDay.Count(l => l.IsOccupied) * 100 / total : (int?)null
                    };
                }).ToList();

                // Dnevna statistika: 7 dana * 24 sata
                var dailyStats = new List<object>();
                foreach (var day in allDays)
                {
                    for (int hour = 0; hour < 24; hour++)
                    {
                        var group = logs.Where(l => l.Timestamp.DayOfWeek == day && l.Timestamp.Hour == hour).ToList();
                        var count = group.Count;
                        dailyStats.Add(new
                        {
                            Day = day.ToString(),
                            Hour = hour,
                            TotalRecords = count,
                            AvgOccupiedPercent = count > 0 ? group.Count(l => l.IsOccupied) * 100 / count : (int?)null
                        });
                    }
                }

                return Ok(new
                {
                    parking.Id,
                    parking.Name,
                    parking.FreeSpotsCount,
                    parking.Occupancy,
                    WeeklyStats = weeklyStats,
                    DailyStats = dailyStats
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška: " + ex.Message);
                return StatusCode(500, "Greška na serveru");
            }
        }
    }
}

